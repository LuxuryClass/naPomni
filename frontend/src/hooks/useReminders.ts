import { useCallback, useEffect, useState } from 'react';
import * as api from '../api/reminders';
import type { Reminder, ReminderDraft } from '../types';

export function useReminders() {
  const [items, setItems] = useState<Reminder[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const reload = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setItems(await api.fetchReminders());
    } catch {
      setError('Не удалось загрузить напоминания. Попробуйте ещё раз.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { void reload(); }, [reload]);

  const create = useCallback(async (draft: ReminderDraft) => {
    const r = await api.createReminder(draft);
    setItems(prev => [r, ...prev]);
  }, []);

  const update = useCallback(async (id: string, patch: Partial<Reminder>) => {
    const r = await api.updateReminder(id, patch);
    setItems(prev => prev.map(x => (x.id === id ? r : x)));
  }, []);

  const remove = useCallback(async (id: string) => {
    await api.deleteReminder(id);
    setItems(prev => prev.filter(x => x.id !== id));
  }, []);

  const togglePin = useCallback(async (id: string) => {
    const current = items.find(x => x.id === id);
    if (!current) return;
    await update(id, { pinned: !current.pinned });
  }, [items, update]);

  return { items, loading, error, reload, create, update, remove, togglePin };
}