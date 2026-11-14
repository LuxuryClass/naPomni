import type { Reminder, ReminderDraft } from '../types';
import { SEED_REMINDERS } from '../mocks';

let store: Reminder[] = [...SEED_REMINDERS];

const delay = (ms = 250) => new Promise<void>(r => setTimeout(r, ms));
const uid = () => Math.random().toString(36).slice(2, 10);

export async function fetchReminders(): Promise<Reminder[]> {
  await delay(400);
  return store.map(r => ({ ...r }));
}

export async function createReminder(draft: ReminderDraft): Promise<Reminder> {
  await delay();
  const r: Reminder = { id: uid(), pinned: false, ...draft };
  store = [r, ...store];
  return { ...r };
}

export async function updateReminder(id: string, patch: Partial<Reminder>): Promise<Reminder> {
  await delay();
  store = store.map(r => (r.id === id ? { ...r, ...patch } : r));
  const found = store.find(r => r.id === id);
  if (!found) throw new Error('Reminder not found');
  return { ...found };
}

export async function deleteReminder(id: string): Promise<void> {
  await delay();
  store = store.filter(r => r.id !== id);
}