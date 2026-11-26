import { type ComponentProps, useMemo, useRef, useState } from 'react';
import { Button, Calendar, Select, Tooltip } from '@skbkontur/react-ui';
import type { Reminder, Team } from '../../../types';
import { formatDayTitle, formatTime, parsePickerDate, toPickerValue } from '../../../utils/date';
import s from './CalendarPanel.module.css';

interface Props {
  reminders: Reminder[];
  teams: Team[];
  onOpenReminder: (reminder: Reminder) => void;
}

type DayProps = Parameters<NonNullable<ComponentProps<typeof Calendar>['renderDay']>>[0];

const MONTHS = [
  [1, 'Январь'], [2, 'Февраль'], [3, 'Март'], [4, 'Апрель'],
  [5, 'Май'], [6, 'Июнь'], [7, 'Июль'], [8, 'Август'],
  [9, 'Сентябрь'], [10, 'Октябрь'], [11, 'Ноябрь'], [12, 'Декабрь'],
] as const;

export function CalendarPanel({ reminders, teams, onOpenReminder }: Props) {
  const calendarRef = useRef<Calendar>(null);
  const [selected, setSelected] = useState(() => toPickerValue(new Date()));
  
  const now = new Date();
  const currentYear = now.getFullYear();

  // Состояние для кастомной шапки (месяц от 1 до 12)
  const [viewMonth, setViewMonth] = useState(() => now.getMonth() + 1);
  const [viewYear, setViewYear] = useState(() => currentYear);

  // Генерируем список лет только в нужном диапазоне (например, ±5 лет)
  const yearItems = useMemo(() => {
    const years: number[] = [];
    for (let y = currentYear - 5; y <= currentYear + 5; y++) {
      years.push(y);
    }
    return years;
  }, [currentYear]);

  const byDay = useMemo(() => {
    const map = new Map<string, Reminder[]>();
    for (const r of reminders) {
      const k = toPickerValue(r.date);
      (map.get(k) ?? map.set(k, []).get(k)!).push(r);
    }
    return map;
  }, [reminders]);

  const teamsMap = new Map(teams.map(t => [t.id, t]));
  const selectedItems = (byDay.get(selected) ?? []).slice().sort((a, b) => a.date.localeCompare(b.date));

  // Переключение месяца или года из кастомной шапки
  const handleNavigate = (month: number, year: number) => {
    setViewMonth(month);
    setViewYear(year);
    calendarRef.current?.scrollToMonth(month, year);
  };

  const goToday = () => {
    const today = new Date();
    const m = today.getMonth() + 1;
    const y = today.getFullYear();
    setSelected(toPickerValue(today));
    handleNavigate(m, y);
  };

  const renderDay = (props: DayProps) => {
    const items = byDay.get(props.date) ?? [];
    const day = (
      <Calendar.Day {...props}>
        <span className={s.dayContent}>
          <span>{+props.date.slice(0, 2)}</span>
          {items.length > 0 && (
            <span className={s.dots}>
              {items.slice(0, 3).map(r => <i key={r.id} className={s.dot} />)}
            </span>
          )}
        </span>
      </Calendar.Day>
    );

    if (!items.length) return day;

    return (
      <Tooltip
        pos="top"
        render={() => (
          <>
            {items.slice(0, 4).map(r => <div key={r.id}>{formatTime(r.date)} — {r.title}</div>)}
            {items.length > 4 && <div>ещё {items.length - 4}…</div>}
          </>
        )}
      >
        {day}
      </Tooltip>
    );
  };

  return (
    <aside className={s.calendar}>
      {/* Кастомная шапка с ограниченным списком лет */}
      <div className={s.customHeader}>
        <Select<number>
          width={120}
          size="small"
          items={MONTHS as unknown as [number, string][]}
          value={viewMonth}
          onValueChange={(m) => handleNavigate(m, viewYear)}
        />
        <Select<number>
          width={85}
          size="small"
          items={yearItems}
          value={viewYear}
          onValueChange={(y) => handleNavigate(viewMonth, y)}
        />
      </div>

      <div className={s.konturWrap}>
        <Calendar
          ref={calendarRef}
          value={selected}
          onValueChange={v => {
            if (!v) return;
            setSelected(v);
            // Синхронизируем шапку, если кликнули по дате соседнего месяца
            const parsed = parsePickerDate(v);
            setViewMonth(parsed.getMonth() + 1);
            setViewYear(parsed.getFullYear());
          }}
          minDate={`01.01.${currentYear - 5}`}
          maxDate={`31.12.${currentYear + 5}`}
          renderDay={renderDay}
        />
      </div>

      <div className={s.dayPanel}>
        <div className={s.dayTitle}>
          <b>{formatDayTitle(parsePickerDate(selected))}</b>
          <Button size="small" onClick={goToday}>Сегодня</Button>
        </div>

        {!selectedItems.length ? (
          <p className={s.empty}>В этот день напоминаний нет.</p>
        ) : (
          <ul className={s.list}>
            {selectedItems.map(r => {
              const team = teamsMap.get(r.teamId);
              return (
                <li key={r.id}>
                  <button
                    type="button"
                    className={s.listItem}
                    aria-haspopup="dialog"
                    onClick={() => onOpenReminder(r)}
                  >
                    <span className={s.listTime}>{formatTime(r.date)}</span>
                    <span className={s.listTitle} title={r.title}>{r.title}</span>
                    {team && <i className={s.dot} style={{ background: team.color }} title={team.name} />}
                  </button>
                </li>
              );
            })}
          </ul>
        )}
      </div>
    </aside>
  );
}
