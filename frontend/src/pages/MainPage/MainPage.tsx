// src/pages/MainPage/MainPage.tsx
import { useEffect, useMemo, useState } from 'react';
import { Add, Calendar, Delete, Search } from '@skbkontur/react-icons';
import { Button, Input, Loader, Toast } from '@skbkontur/react-ui';
import { CalendarPanel } from '../../components/calendar/CalendarPanel/CalendarPanel';
import { Sidebar } from '../../components/layout/Sidebar/Sidebar';
import { ReminderCard } from '../../components/reminders/ReminderCard/ReminderCard';
import { ReminderFormModal } from '../../components/reminders/ReminderFormModal/ReminderFormModal';
import { useReminders } from '../../hooks/useReminders';
import { CURRENT_USER, TEAMS } from '../../mocks';
import type { Reminder, ReminderDraft } from '../../types';
import type { ReminderGroup } from '../../utils/date';
import { getGroup } from '../../utils/date';
import s from './MainPage.module.css';

const GROUPS: { key: ReminderGroup; title: string }[] = [
  { key: 'urgent', title: 'Горящие' },
  { key: 'current', title: 'Актуальные' },
  { key: 'past', title: 'Прошедшие' },
];

interface ToastState {
  id: number;
  text: string;
}

export function MainPage() {
  const { items, loading, error, reload, create, update, remove, togglePin } = useReminders();
  const [expanded, setExpanded] = useState(false);
  const [activeTeamId, setActiveTeamId] = useState<string | null>(null);
  const [query, setQuery] = useState('');
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<Reminder | null>(null);
  const [toast, setToast] = useState<ToastState | null>(null);

  // Наш заменитель Toast.push: показываем и сами прячем через 4 секунды
  const show = (text: string) => setToast({ id: Date.now(), text });

  useEffect(() => {
    if (!toast) return;
    const timer = setTimeout(() => setToast(null), 4000);
    return () => clearTimeout(timer);
  }, [toast]);

  const activeTeam = TEAMS.find(t => t.id === activeTeamId) ?? null;

  const visible = useMemo(() => {
    const q = query.trim().toLowerCase();
    return items.filter(r =>
      (!activeTeamId || r.teamId === activeTeamId) &&
      (!q || r.title.toLowerCase().includes(q) || r.description.toLowerCase().includes(q)),
    );
  }, [items, activeTeamId, query]);

  const grouped = useMemo(() => {
    const g: Record<ReminderGroup, Reminder[]> = { urgent: [], current: [], past: [] };
    for (const r of visible) g[getGroup(r)].push(r);
    g.urgent.sort((a, b) => a.date.localeCompare(b.date));
    g.current.sort((a, b) => a.date.localeCompare(b.date));
    g.past.sort((a, b) => b.date.localeCompare(a.date));
    return g;
  }, [visible]);

  const openCreate = () => { setEditing(null); setFormOpen(true); };
  const openEdit = (r: Reminder) => { setEditing(r); setFormOpen(true); };

  const handleSubmit = async (draft: ReminderDraft) => {
    try {
      if (editing) {
        await update(editing.id, draft);
        show('Напоминание обновлено');
      } else {
        await create(draft);
        show('Напоминание добавлено');
      }
    } catch (e) {
      show('Не удалось сохранить напоминание');
      throw e;
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await remove(id);
      show('Напоминание удалено');
    } catch {
      show('Не удалось удалить напоминание');
    }
  };

  const handleTogglePin = async (id: string) => {
    try {
      await togglePin(id);
    } catch {
      show('Не удалось изменить закрепление');
    }
  };

  return (
    <div className={s.app}>
      <Sidebar
        expanded={expanded}
        onToggle={() => setExpanded(v => !v)}
        teams={TEAMS}
        activeTeamId={activeTeamId}
        onSelectTeam={setActiveTeamId}
        user={CURRENT_USER}
        onProfile={() => show('Профиль появится в следующей итерации')}
      />

      {/* Клик по правой части сворачивает раскрытую панель */}
      <main className={s.content} onClick={() => expanded && setExpanded(false)}>
        <header className={s.header}>
          <div className={s.headerTop}>
            <h1>Напоминания</h1>
            {activeTeam && (
              <span className={s.teamFilter}>
                <i style={{ background: activeTeam.color }} />
                {activeTeam.name}
                <button onClick={() => setActiveTeamId(null)} title="Сбросить фильтр"><Delete size={12} /></button>
              </span>
            )}
            <div className={s.spacer} />
            <Button use="primary" icon={<Add size={14} />} onClick={openCreate}>Добавить напоминание</Button>
          </div>
          <Input
            value={query}
            onValueChange={setQuery}
            placeholder="Поиск по напоминаниям"
            leftIcon={<Search size={14} />}
            width={360}
          />
        </header>

        {loading ? (
          <div className={s.state}>
            <Loader active caption="Загружаем напоминания"><div className={s.stateBox} /></Loader>
          </div>
        ) : error ? (
          <div className={s.state}>
            <p>{error}</p>
            <Button onClick={reload}>Повторить</Button>
          </div>
        ) : (
          <div className={s.columns}>
            <section className={s.list}>
              {visible.length === 0 ? (
                <div className={s.state}>
                  <Calendar size={32} color="#9AA0A8" />
                  <p>{items.length === 0 ? 'Пока нет ни одного напоминания.' : 'Ничего не найдено. Измените запрос или фильтр.'}</p>
                  <Button use="primary" icon={<Add size={14} />} onClick={openCreate}>Добавить напоминание</Button>
                </div>
              ) : (
                GROUPS.map(({ key, title }) =>
                  grouped[key].length > 0 && (
                    <section className={s.group} key={key}>
                      <h2 className={`${s.groupTitle}${key === 'urgent' ? ` ${s.groupTitleUrgent}` : ''}`}>
                        {title} <span>{grouped[key].length}</span>
                      </h2>
                      <div className={s.groupItems}>
                        {grouped[key].map((r, i) => (
                          <ReminderCard
                            key={r.id}
                            reminder={r}
                            team={TEAMS.find(t => t.id === r.teamId)}
                            index={i}
                            onTogglePin={handleTogglePin}
                            onEdit={openEdit}
                            onDelete={handleDelete}
                          />
                        ))}
                      </div>
                    </section>
                  ),
                )
              )}
            </section>

            <CalendarPanel reminders={visible} teams={TEAMS} />
          </div>
        )}
      </main>

      {formOpen && (
        <ReminderFormModal
          teams={TEAMS}
          initial={editing}
          defaultTeamId={activeTeamId ?? TEAMS[0].id}
          onClose={() => setFormOpen(false)}
          onSubmit={handleSubmit}
        />
      )}

      {toast && (
        <Toast key={toast.id} onClose={() => setToast(null)}>
          {toast.text}
        </Toast>
      )}
    </div>
  );
}