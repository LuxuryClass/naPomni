// src/components/reminders/ReminderFormModal.tsx
import { useMemo, useState } from 'react';
import { Button, DatePicker, Gapped, Input, Modal, Select, Textarea } from '@skbkontur/react-ui';
import type { Reminder, ReminderDraft, Team } from '../../../types';
import { fromPickerValue, toPickerValue } from '../../../utils/date';
import s from './ReminderFormModal.module.css';

interface Props {
  teams: Team[];
  initial: Reminder | null;
  defaultTeamId: string;
  onClose: () => void;
  onSubmit: (draft: ReminderDraft) => Promise<void>;
}

export function ReminderFormModal({ teams, initial, defaultTeamId, onClose, onSubmit }: Props) {
  const [title, setTitle] = useState(initial?.title ?? '');
  const [date, setDate] = useState(initial ? toPickerValue(initial.date) : '');
  const [teamId, setTeamId] = useState(initial?.teamId ?? defaultTeamId);
  const [description, setDescription] = useState(initial?.description ?? '');
  const [errors, setErrors] = useState<{ title?: boolean; date?: boolean; dateText?: string }>({});
  const [saving, setSaving] = useState(false);

  const teamItems = useMemo<[string, string][]>(() => teams.map(t => [t.id, t.name]), [teams]);

  const submit = async () => {
    const iso = fromPickerValue(date);
    const next: typeof errors = {};
    if (!title.trim()) next.title = true;
    if (!iso) {
      next.date = true;
      next.dateText = 'Введите дату в формате ДД.ММ.ГГГГ';
    } else if (!initial && new Date(iso).getTime() < Date.now()) {
      next.date = true;
      next.dateText = 'Дата уже прошла';
    }
    setErrors(next);
    if (next.title || next.date || !iso) return;

    setSaving(true);
    try {
      await onSubmit({ title: title.trim(), description: description.trim(), date: iso, teamId });
      onClose();
    } catch {
      // ошибку покажет родитель через Toast
    } finally {
      setSaving(false);
    }
  };

  return (
    <Modal onClose={onClose} width={520}>
      <Modal.Header>{initial ? 'Редактировать напоминание' : 'Новое напоминание'}</Modal.Header>
      <Modal.Body>
        <div className={s.form}>
          <label className={s.field}>
            <span className={s.label}>Название *</span>
            <Input value={title} onValueChange={setTitle} placeholder="Например, продлить лицензии" width="100%" maxLength={200} error={errors.title} />
          </label>
          <label className={s.field}>
            <span className={s.label}>Дата *</span>
            <DatePicker value={date} onValueChange={setDate} width="100%" error={errors.date} />
            {errors.dateText
              ? <span className={s.error}>{errors.dateText}</span>
              : <span className={s.hint}>Сработает в 10:00 по вашему времени</span>}
          </label>
          <label className={s.field}>
            <span className={s.label}>Команда</span>
            <Select<string> items={teamItems} value={teamId} onValueChange={setTeamId} width="100%" />
          </label>
          <label className={s.field}>
            <span className={s.label}>Описание</span>
            <Textarea value={description} onValueChange={setDescription} rows={3} width="100%" placeholder="Подробности — их увидит только команда" />
          </label>
        </div>
      </Modal.Body>
      <Modal.Footer panel>
        <Gapped gap={8}>
          <Button use="primary" loading={saving} onClick={submit}>Сохранить</Button>
          <Button onClick={onClose}>Отмена</Button>
        </Gapped>
      </Modal.Footer>
    </Modal>
  );
}