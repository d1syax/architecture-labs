# Autistic Bank API — Lab 2

Clean Architecture рефакторинг лаби 1. 4 окремих шари з інверсією залежностей
та Rich Domain Model.

## Структура
MyBank.Domain        ← бізнес-логіка, інтерфейси, доменні помилки
MyBank.Application   ← use cases, оркестрація
MyBank.Infrastructure ← EF Core, репозиторії, маппери, JWT
MyBank.Api           ← контролери, DTO

## Запуск

1. Налаштуй `MyBank.Api/appsettings.json` — вкажи пароль PostgreSQL
2. Застосуй міграцію:

```bash
cd MyBank.Api
dotnet ef database update --project ../MyBank.Infrastructure/MyBank.Infrastructure.csproj
```

3. Запусти:

```bash
dotnet run
```

## Тести

```bash
cd MyBank.Tests
dotnet test
```

## Ендпоінти

| Method | URL | Auth | Опис |
|--------|-----|------|------|
| POST | /api/auth/register | — | Реєстрація |
| POST | /api/auth/login | — | Логін |
| POST | /api/accounts | JWT | Створення рахунку |
| GET | /api/accounts | JWT | Мої рахунки |
| POST | /api/accounts/transfer | JWT | Переказ |
| POST | /api/accounts/{id}/deposit | JWT | Поповнення |