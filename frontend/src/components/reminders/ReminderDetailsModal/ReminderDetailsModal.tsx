import { useEffect, useId, useRef, useState } from 'react';
import { Button } from '@skbkontur/react-ui';
import type { Reminder, Team } from '../../../types';
import { formatFull, formatTime } from '../../../utils/date';
import s from './ReminderDetailsModal.module.css';

const shortMonth = new Intl.DateTimeFormat('ru-RU', { month: 'short' });

interface Props {
  reminder: Reminder;
  team?: Team;
  onClose: () => void;
  onEdit: (reminder: Reminder) => void;
  onDelete: (id: string) => Promise<void>;
}

export function ReminderDetailsModal({ reminder, team, onClose, onEdit, onDelete }: Props) {
  const dialog = useRef<HTMLDialogElement>(null);
  const titleId = useId();
  const [confirmDelete, setConfirmDelete] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const date = new Date(reminder.date);
  const elapsed = date.getTime() < Date.now();

  useEffect(() => {
    const element = dialog.current;
    if (element && !element.open) element.showModal();
  }, []);

  const close = () => dialog.current?.close();

  const deleteReminder = async () => {
    setDeleting(true);
    await onDelete(reminder.id);
    setDeleting(false);
  };

  return (
    <dialog
      ref={dialog}
      className={s.dialog}
      aria-labelledby={titleId}
      onClose={onClose}
      onClick={event => {
        if (event.target === event.currentTarget) close();
      }}
    >
      <div className={s.panel}>
        <button type="button" className={s.close} aria-label="Закрыть" onClick={close}>×</button>
        <header className={s.header}>
          <time className={s.date} dateTime={reminder.date} title={formatFull(reminder.date)}>
            <b>{date.getDate()}</b>
            <span>{shortMonth.format(date).replace(/\.$/, '')}</span>
          </time>
          <div className={s.summary}>
            <h2 id={titleId}>{reminder.title}</h2>
            <span className={elapsed ? `${s.status} ${s.elapsed}` : s.status}>
              {elapsed ? 'Время прошло' : 'Ожидает'}
            </span>
            <div className={s.metadata}>
              {team && <span><i className="dot" style={{ background: team.color }} />{team.name}</span>}
              <span>{formatTime(reminder.date)}</span>
            </div>
          </div>
        </header>

        {reminder.description && <p className={s.description}>{reminder.description}</p>}

        <footer className={s.actions}>
          {confirmDelete ? (
            <>
              <Button use="danger" loading={deleting} disabled={deleting} onClick={deleteReminder}>Точно удалить?</Button>
              <Button disabled={deleting} onClick={() => setConfirmDelete(false)}>Отмена</Button>
            </>
          ) : (
            <>
              <Button onClick={() => { close(); onEdit(reminder); }}>Редактировать</Button>
              <Button onClick={() => setConfirmDelete(true)}>Удалить</Button>
            </>
          )}
        </footer>
      </div>
    </dialog>
  );
}
