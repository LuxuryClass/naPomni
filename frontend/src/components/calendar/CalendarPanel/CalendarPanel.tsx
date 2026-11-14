// src/components/calendar/CalendarPanel.tsx
import { useMemo, useState } from 'react';
import { ArrowChevronLeft, ArrowChevronRight  } from '@skbkontur/react-icons';
import { Button, Tooltip } from '@skbkontur/react-ui';
import type { Reminder, Team } from '../../../types';
import { buildCalendar, dayKey, formatDayTitle, formatTime, getGroup, isSameDay, MONTHS, WEEKDAYS } from '../../../utils/date';
import s from './CalendarPanel.module.css';

interface Props {
  reminders: Reminder[];
  teams: Team[];
}

export function CalendarPanel({ reminders, teams }: Props) {
  const today = new Date();
  const [month, setMonth] = useState(() => new Date(today.getFullYear(), today.getMonth(), 1));
  const [selected, setSelected] = useState<Date>(today);

  const byDay = useMemo(() => {
    const map = new Map<string, Reminder[]>();
    for (const r of reminders) {
      const key = dayKey(new Date(r.date));
      map.set(key, [...(map.get(key) ?? []), r]);
    }
    return map;
  }, [reminders]);

  const cells = useMemo(() => buildCalendar(month), [month]);
  const selectedItems = (byDay.get(dayKey(selected)) ?? []).slice().sort((a, b) => a.date.localeCompare(b.date));
  const shift = (d: number) => setMonth(m => new Date(m.getFullYear(), m.getMonth() + d, 1));
  const goToday = () => {
    setMonth(new Date(today.getFullYear(), today.getMonth(), 1));
    setSelected(today);
  };

  return (
    <aside className={s.calendar}>
      <div className={s.header}>
        <Button size="small" icon={<ArrowChevronLeft size={14} />} onClick={() => shift(-1)} />
        <b>{MONTHS[month.getMonth()]} {month.getFullYear()}</b>
        <Button size="small" icon={<ArrowChevronRight  size={14} />} onClick={() => shift(1)} />
      </div>

      <div className={s.weekdays}>
        {WEEKDAYS.map(w => <span key={w}>{w}</span>)}
      </div>

      <div className={s.grid}>
        {cells.map(d => {
          const items = byDay.get(dayKey(d)) ?? [];
          const cls = [s.day];
          if (d.getMonth() !== month.getMonth()) cls.push(s.muted);
          if (isSameDay(d, today)) cls.push(s.today);
          if (isSameDay(d, selected)) cls.push(s.selected);

          const cell = (
            <button className={cls.join(' ')} onClick={() => setSelected(d)}>
              <span>{d.getDate()}</span>
              {items.length > 0 && (
                <span className={s.dots}>
                  {items.slice(0, 3).map(r => <i key={r.id} className={`dot dot--${getGroup(r)}`} />)}
                </span>
              )}
            </button>
          );

          return items.length > 0 ? (
            <Tooltip
              key={dayKey(d)}
              pos="top"
              render={() => (
                <div>
                  {items.slice(0, 4).map(r => <div key={r.id}>{formatTime(r.date)} — {r.title}</div>)}
                  {items.length > 4 && <div>ещё {items.length - 4}…</div>}
                </div>
              )}
            >
              {cell}
            </Tooltip>
          ) : (
            <span key={dayKey(d)}>{cell}</span>
          );
        })}
      </div>

      <div className={s.legend}>
        <span><i className="dot dot--urgent" /> Горящие</span>
        <span><i className="dot dot--current" /> Актуальные</span>
        <span><i className="dot dot--past" /> Прошедшие</span>
      </div>

      <div className={s.dayPanel}>
        <div className={s.dayTitle}>
          <b>{formatDayTitle(selected)}</b>
          <Button size="small" onClick={goToday}>Сегодня</Button>
        </div>
        {selectedItems.length === 0 ? (
          <p className={s.empty}>В этот день напоминаний нет.</p>
        ) : (
          <ul className={s.list}>
            {selectedItems.map(r => {
              const team = teams.find(t => t.id === r.teamId);
              return (
                <li key={r.id}>
                  <span className={s.listTime}>{formatTime(r.date)}</span>
                  <span className={s.listTitle}>{r.title}</span>
                  {team && <i className="dot" style={{ background: team.color }} title={team.name} />}
                </li>
              );
            })}
          </ul>
        )}
      </div>
    </aside>
  );
}