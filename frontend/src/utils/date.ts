import type { Reminder } from '../types';

export type ReminderGroup = 'today' | 'week' | 'later' | 'archive';

export const MONTHS_GEN = ['января', 'февраля', 'марта', 'апреля', 'мая', 'июня', 'июля', 'августа', 'сентября', 'октября', 'ноября', 'декабря'];

const pad = (n: number) => String(n).padStart(2, '0');

export function getGroup(r: Reminder, now: Date = new Date()): ReminderGroup {
  const t = new Date(r.date).getTime();
  const startOfToday = new Date(now);
  startOfToday.setHours(0, 0, 0, 0);
  if (t < startOfToday.getTime()) return 'archive';

  const startOfTomorrow = new Date(startOfToday);
  startOfTomorrow.setDate(startOfToday.getDate() + 1);
  if (t < startOfTomorrow.getTime()) return 'today';

  const startOfNextWeek = new Date(startOfToday);
  const daysUntilMonday = 7 - (startOfToday.getDay() + 6) % 7;
  startOfNextWeek.setDate(startOfToday.getDate() + daysUntilMonday);
  return t < startOfNextWeek.getTime() ? 'week' : 'later';
}

export function formatTime(iso: string): string {
  const d = new Date(iso);
  return `${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

export function isSameDay(a: Date, b: Date): boolean {
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate();
}

export function formatDayLabel(iso: string): string {
  const d = new Date(iso);
  const now = new Date();
  const tomorrow = new Date(now);
  tomorrow.setDate(now.getDate() + 1);
  if (isSameDay(d, now)) return 'Сегодня';
  if (isSameDay(d, tomorrow)) return 'Завтра';
  return `${d.getDate()} ${MONTHS_GEN[d.getMonth()]}`;
}

export function formatFull(iso: string): string {
  const d = new Date(iso);
  return `${d.getDate()} ${MONTHS_GEN[d.getMonth()]} ${d.getFullYear()}, ${formatTime(iso)}`;
}

export function formatDayTitle(d: Date): string {
  return `${d.getDate()} ${MONTHS_GEN[d.getMonth()]} ${d.getFullYear()}`;
}

// Теперь принимает и строку ISO, и объект Date
export function toPickerValue(input: string | Date): string {
  const d = typeof input === 'string' ? new Date(input) : input;
  return `${pad(d.getDate())}.${pad(d.getMonth() + 1)}.${d.getFullYear()}`;
}

export function parsePickerDate(value: string): Date {
  const [day, month, year] = value.split('.').map(Number);
  return new Date(year, month - 1, day);
}

export function fromPickerValue(value: string, hour = 10): string | null {
  const m = /^(\d{2})\.(\d{2})\.(\d{4})$/.exec(value.trim());
  if (!m) return null;
  const day = +m[1], month = +m[2], year = +m[3];
  const d = new Date(year, month - 1, day, hour, 0, 0, 0);
  if (d.getFullYear() !== year || d.getMonth() !== month - 1 || d.getDate() !== day) return null;
  return d.toISOString();
}

export function dayKey(d: Date): string {
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
}
