# НаПомни
Веб-приложение для работы с командными напоминаниями. На главной странице можно создавать, редактировать, удалять и закреплять напоминания, искать их по названию или описанию, фильтровать по команде и просматривать в календаре.

## Запуск интерфейса
Понадобятся Node.js и npm. Из корня проекта выполните:

```bash
cd frontend
npm ci
npm run dev
```

Откройте адрес, который выведет Vite (обычно http://localhost:5173). Для сборки используйте `npm run build`, для просмотра сборки - `npm run preview`.

## Запуск серверной части
Понадобятся .NET SDK 10.0 или новее и Docker с Docker Compose. Команды ниже выполняются в PowerShell из корня проекта.

1. Запустите PostgreSQL:

   ```powershell
   docker compose -f deploy/docker-compose.yml up -d postgres
   ```

   База будет доступна на `localhost:5433`; имя базы, пользователь и пароль - `reminders`.

2. В том же терминале задайте строку подключения и запустите API:

   ```powershell
   $env:databaseUrl = 'Host=localhost;Port=5433;Database=reminders;Username=reminders;Password=reminders'
   dotnet run --project backend/src/Reminders.Api --no-launch-profile --urls http://localhost:5000
   ```

3. В отдельном терминале задайте ту же строку подключения и запустите worker:

   ```powershell
   $env:databaseUrl = 'Host=localhost;Port=5433;Database=reminders;Username=reminders;Password=reminders'
   dotnet run --project backend/src/Reminders.Worker
   ```

API будет доступен на `http://localhost:5000` - этот адрес указан в настройках прокси Vite. Остановите API и worker с помощью `Ctrl+C`, а PostgreSQL - командой `docker compose -f deploy/docker-compose.yml down` из корня проекта. Данные базы сохраняются в Docker volume.

## Состояние проекта
Интерфейс пока использует тестовые команды, профиль и напоминания. Изменения хранятся только в памяти страницы и сбрасываются при её обновлении. Серверная часть и отправка уведомлений находятся в разработке: контроллеры API и фоновые задания пока не реализованы. В `deploy/docker-compose.yml` подготовлена конфигурация PostgreSQL, API и worker; интерфейс в неё не входит.

Проект состоит из `frontend` (React, TypeScript, Vite), `backend` (.NET) и `deploy` (Docker Compose). Описание планируемой архитектуры находится в `docs/backend-architecture.txt`.
