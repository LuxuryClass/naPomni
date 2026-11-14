export interface Reminder {
  id: string;
  title: string;
  description: string;
  date: string;
  teamId: string;
  pinned: boolean;
}

export interface ReminderDraft {
  title: string;
  description: string;
  date: string;
  teamId: string;
}