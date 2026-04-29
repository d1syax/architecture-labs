# Лабораторна робота 2: Шарова архітектура та доменна модель
*Done by: YOUR_NAME IM-XX*

## Prerequisites
- .NET 9 SDK
- PostgreSQL 15+

## Installation

Clone the repository:
```bash
git clone https://github.com/YOUR_USERNAME/architecture-labs.git
cd architecture-labs/lab2
```

## Configuration

Open `MyBank.Api/appsettings.json` and set your PostgreSQL credentials:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=autistic_bank_lab2;Username=postgres;Password=YOUR_PASSWORD"
}
```

## Running

```bash
cd MyBank.Api
dotnet run
```

## Migrations

```bash
cd MyBank.Api
dotnet ef database update --project ../MyBank.Infrastructure/MyBank.Infrastructure.csproj
```

## Testing

```bash
cd MyBank.Tests
dotnet test
```

## Project Structure
MyBank.Domain           ← доменні моделі, інтерфейси репозиторіїв, доменні помилки
MyBank.Application      ← сервіси, use cases, оркестрація
MyBank.Infrastructure   ← EF Core, репозиторії, маппери, JWT
MyBank.Api              ← контролери, DTO
MyBank.Tests            ← unit та integration тести

## Endpoints

| Method | URL | Auth | Description |
|--------|-----|------|-------------|
| POST | /api/auth/register | — | Register new user |
| POST | /api/auth/login | — | Login and get JWT token |
| POST | /api/accounts | JWT | Create bank account |
| GET | /api/accounts | JWT | Get my accounts |
| POST | /api/accounts/transfer | JWT | Transfer funds |
| POST | /api/accounts/{id}/deposit | JWT | Deposit funds |