import { Pin } from '@skbkontur/react-icons';
import { Button, Tooltip } from '@skbkontur/react-ui';
import type { Reminder, Team } from '../../../types';
import { formatDayLabel, formatTime } from '../../../utils/date';
import s from './ReminderCard.module.css';

interface Props {
  reminder: Reminder;
  team?: Team;
  index: number;
  onTogglePin: (id: string) => void;
  onOpen: (reminder: Reminder) => void;
}

export function ReminderCard({ reminder, team, index, onTogglePin, onOpen }: Props) {
  const pinLabel = reminder.pinned ? 'Открепить' : 'Закрепить';

  return (
    <article className={s.card} style={{ animationDelay: `${Math.min(index, 8) * 40}ms` }}>
      <button type="button" className={s.main} aria-haspopup="dialog" onClick={() => onOpen(reminder)}>
        <span className={s.date}>
          <b>{formatDayLabel(reminder.date)}</b>
          <span>{formatTime(reminder.date)}</span>
        </span>

        <span className={s.body}>
          <span className={s.title}>
            {reminder.pinned && <Pin size={14} color="#E5484D" />}
            <span>{reminder.title}</span>
          </span>
          {team && (
            <span className={s.teamChip}><i style={{ background: team.color }} />{team.name}</span>
          )}
        </span>
      </button>

      <div className={s.actions}>
        <Tooltip pos="top" render={() => pinLabel}>
          <Button
            size="small"
            aria-label={pinLabel}
            icon={<Pin size={14} color={reminder.pinned ? '#E5484D' : undefined} />}
            onClick={() => onTogglePin(reminder.id)}
          />
        </Tooltip>
      </div>
    </article>
  );
}
