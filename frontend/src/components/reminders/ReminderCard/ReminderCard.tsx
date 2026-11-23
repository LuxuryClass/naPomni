// src/components/reminders/ReminderCard.tsx
import { useState } from 'react';
import { Clock, Delete, Edit, Pin } from '@skbkontur/react-icons';
import { Button, Gapped, Tooltip } from '@skbkontur/react-ui';
import type { Reminder, Team } from '../../../types';
import { formatDayLabel, formatFull, formatTime } from '../../../utils/date';
import s from './ReminderCard.module.css';

interface Props {
  reminder: Reminder;
  team?: Team;
  index: number;
  onTogglePin: (id: string) => void;
  onEdit: (reminder: Reminder) => void;
  onDelete: (id: string) => void;
}

export function ReminderCard({ reminder, team, index, onTogglePin, onEdit, onDelete }: Props) {
  const [open, setOpen] = useState(false);
  const [confirmDelete, setConfirmDelete] = useState(false);
  return (
    <article className={s.card} style={{ animationDelay: `${Math.min(index, 8) * 40}ms` }}>
      <div className={s.main}>
        <div className={s.date}>
          <b>{formatDayLabel(reminder.date)}</b>
          <span>{formatTime(reminder.date)}</span>
        </div>

        <div className={s.body}>
          <div className={s.title}>
            {reminder.pinned && <Pin size={14} color="#E5484D" />}
            <span>{reminder.title}</span>
          </div>
          <div className={s.meta}>
            {team && (
              <span className={s.teamChip}><i style={{ background: team.color }} />{team.name}</span>
            )}
          </div>
        </div>

        <div className={s.actions}>
          <Tooltip pos="top" render={() => (reminder.pinned ? 'Открепить' : 'Закрепить')}>
            <Button
              size="small"
              icon={<Pin size={14} color={reminder.pinned ? '#E5484D' : undefined} />}
              onClick={() => onTogglePin(reminder.id)}
            />
          </Tooltip>
          <Button size="small" onClick={() => setOpen(v => !v)}>
            {open ? 'Свернуть' : 'Подробнее'}
          </Button>
        </div>
      </div>

      {open && (
        <div className={s.details}>
          {reminder.description && <p className={s.description}>{reminder.description}</p>}
          <div className={s.fact}><Clock size={14} /> {formatFull(reminder.date)}</div>
          {team && <div className={s.fact}><i className="dot" style={{ background: team.color }} /> Команда: {team.name}</div>}
          <div className={s.detailActions}>
            <Gapped gap={8}>
              <Button size="small" icon={<Edit size={14} />} onClick={() => onEdit(reminder)}>Редактировать</Button>
              {confirmDelete ? (
                <>
                  <Button size="small" use="danger" icon={<Delete size={14} />} onClick={() => onDelete(reminder.id)}>
                    Точно удалить?
                  </Button>
                  <Button size="small" onClick={() => setConfirmDelete(false)}>Отмена</Button>
                </>
              ) : (
                <Button size="small" icon={<Delete size={14} />} onClick={() => setConfirmDelete(true)}>Удалить</Button>
              )}
            </Gapped>
          </div>
        </div>
      )}
    </article>
  );
}
