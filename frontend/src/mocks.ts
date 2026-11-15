// src/mocks.ts
import type { Reminder, Team, UserProfile } from './types';

export const TEAMS: Team[] = [
  { id: 'platform',   name: 'Платформа',   short: 'П',  color: 'var(--team-blue)' },
  { id: 'accounting', name: 'Бухгалтерия', short: 'Б',  color: 'var(--team-purple)' },
  { id: 'it',         name: 'ИТ',          short: 'И',  color: 'var(--team-green)' },
  { id: 'marketing',  name: 'Маркетинг',   short: 'М',  color: 'var(--team-orange)' },
  { id: 'sales',      name: 'Продажи',     short: 'П',  color: 'var(--team-teal)' },
  { id: 'hr',         name: 'HR',          short: 'HR', color: 'var(--team-pink)' },
];

export const CURRENT_USER: UserProfile = {
  name: 'Абоба',
  email: 'aboba@kontur.ru',
  id: '1',
};

const at = (days: number, hour: number, minute = 0): string => {
  const d = new Date();
  d.setDate(d.getDate() + days);
  d.setHours(hour, minute, 0, 0);
  return d.toISOString();
};

export const SEED_REMINDERS: Reminder[] = [
  { id: 'r1', title: 'Отправить отчёт по SLA', description: 'Квартальный отчёт по SLA: собрать метрики у дежурных и выложить в общий чат.', date: at(1, 9), teamId: 'platform', pinned: false },
  { id: 'r2', title: 'Продлить лицензии Figma', description: 'Оплатить продление на год, пока редакторам не отключили доступ.', date: at(2, 10), teamId: 'it', pinned: false },
  { id: 'r3', title: 'Квартальная сверка расходов', description: 'Сверить бюджет команды с бухгалтерией до закрытия квартала.', date: at(21, 11), teamId: 'accounting', pinned: true },
  { id: 'r4', title: 'Проверить резервные копии', description: 'Убедиться, что ночные бэкапы восстанавливаются на тестовом стенде.', date: at(4, 9, 30), teamId: 'it', pinned: false },
  { id: 'r5', title: 'Обновить регламенты', description: 'Актуализировать регламенты дежурств и эскалаций.', date: at(8, 15), teamId: 'platform', pinned: false },
  { id: 'r6', title: 'Ревизия доступов', description: 'Проверить доступы подрядчиков, отключить неактивные учётные записи.', date: at(13, 12), teamId: 'hr', pinned: false },
  { id: 'r7', title: 'Запустить опрос команды', description: 'Ежеквартальный опрос удовлетворённости процессами.', date: at(27, 10), teamId: 'hr', pinned: false },
  { id: 'r8', title: 'Обновить прайс-лист', description: 'Согласовать новые цены с продажами и опубликовать.', date: at(35, 14), teamId: 'sales', pinned: false },
  { id: 'r9', title: 'Обновить SSL-сертификаты', description: 'Перевыпустить сертификаты на внутренних доменах.', date: at(-2, 10), teamId: 'it', pinned: false },
  { id: 'r10', title: 'Подготовить ретро', description: 'Собрать темы для ретроспективы спринта.', date: at(-6, 16), teamId: 'platform', pinned: false },
  { id: 'r11', title: 'Свести отчёт по найму', description: 'Отчёт для HR-директора по воронке найма за месяц.', date: at(-15, 11), teamId: 'hr', pinned: false },
];