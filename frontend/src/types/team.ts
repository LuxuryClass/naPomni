export interface Team {
  id: string;
  name: string;
  short: string;     // буква, если картинки нет
  color: string;
  iconUrl?: string;  // путь к картинке команды
}