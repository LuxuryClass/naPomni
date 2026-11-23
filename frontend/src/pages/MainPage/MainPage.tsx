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
import { getGroup } from '../../utils/date';
import s from './MainPage.module.css';

const GROUPS = [
  { key: 'today', title: 'Сегодня' },
  { key: 'week', title: 'Эта неделя' },
  { key: 'later', title: 'Далее' },
] as const;

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
  const [today, setToday] = useState(() => new Date());

  useEffect(() => {
    const now = new Date();
    const tomorrow = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1);
    const timer = setTimeout(() => setToday(new Date()), Math.max(0, tomorrow.getTime() - now.getTime()));
    return () => clearTimeout(timer);
  }, [today]);

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
    const groups: Record<(typeof GROUPS)[number]['key'], Reminder[]> = { today: [], week: [], later: [] };
    const sorted = [...visible].sort((a, b) => Date.parse(a.date) - Date.parse(b.date));
    for (const reminder of sorted) {
      const group = getGroup(reminder, today);
      if (group !== 'archive') groups[group].push(reminder);
    }
    return groups;
  }, [visible, today]);
  const activeReminders = useMemo(() => GROUPS.flatMap(({ key }) => grouped[key]), [grouped]);

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

  const renderCards = (reminders: Reminder[]) =>
    reminders.map((reminder, index) => (
      <ReminderCard
        key={reminder.id}
        reminder={reminder}
        team={TEAMS.find(team => team.id === reminder.teamId)}
        index={index}
        onTogglePin={handleTogglePin}
        onEdit={openEdit}
        onDelete={handleDelete}
      />
    ));

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
                    <section className={key === 'today' ? s.todayGroup : undefined} key={key}>
                      <h2 className={s.groupTitle}>
                        {title} <span>{grouped[key].length}</span>
                      </h2>
                      <div className={s.groupItems}>
                        {renderCards(grouped[key])}
                      </div>
                    </section>
                  ),
                )
              )}
              <div>
                <Button disabled>Архив задач</Button>
              </div>
            </section>

            <CalendarPanel reminders={activeReminders} teams={TEAMS} />
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
