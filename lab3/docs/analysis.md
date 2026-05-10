# Аналіз: Лабораторна робота 3

## Що змінилося в структурі проєкту порівняно з лабораторною 2?

В лабораторній роботі 2 Application Layer містив два сервіси - `AccountService` та `AuthService`.
Кожен сервіс мав кілька методів і всі залежності в конструкторі, навіть якщо конкретний метод
їх не використовував.

В лабораторній роботі 3 сервіси повністю замінені на окремі Command та Query Handler'и.

З'явився окремий інтерфейс `IAccountReadRepository` для читання - живе в Application Layer
і повертає `AccountReadModel` (DTO), а не доменні моделі. Реалізація читає напряму з БД
через EF Core без маппера і доменного шару.

Додано MediatR як медіатор між контролером і хендлером. Контролери більше не знають
про конкретні хендлери - вони відправляють команди і запити через `IMediator`:

```csharp
// Лаба 2
var result = await _accountService.CreateAsync(GetUserId(), request.Currency);

// Лаба 3
var result = await _mediator.Send(new CreateAccountCommand(GetUserId(), request.Currency));
```

## Переваги CQS

**Один Handler - одна відповідальність і мінімум залежностей.**
`TransferCommandHandler` залежить тільки від `IAccountRepository`.
`GetMyAccountsQueryHandler` залежить тільки від `IAccountReadRepository`.
В лабі 2 `AccountService` мав обидві залежності навіть якщо метод використовував тільки одну.

**Читання не проходить через домен.**
`GetMyAccountsQueryHandler` читає напряму з БД і повертає `AccountReadModel`.
Не потрібно завантажувати доменні об'єкти, викликати маппер, конвертувати Value Objects.
Це простіше і ефективніше - EF Core генерує точний SQL тільки з потрібними полями.

**Додати нову операцію = створити новий Handler.**
Не потрібно чіпати існуючий код. Новий `WithdrawCommand` + `WithdrawCommandHandler`
і існуючий код не змінюється. Це відповідає Open/Closed Principle.

**Тестування стало фокусованішим.**
`CreateAccountCommandHandlerTests` тестує тільки створення акаунту з мінімумом моків.
В лабі 2 щоб протестувати один метод потрібно було мокати всі залежності сервісу.

**Контролери повністю тонкі.**
Єдина відповідальність контролера - маппінг HTTP запиту в Command або Query і передача
через `IMediator`. Жодної логіки, жодних залежностей від домену.

## Недоліки CQS

**Більше файлів.**
Три методи в сервісі перетворились на дев'ять файлів - три команди, три хендлери,
один запит, один хендлер, один інтерфейс. Навігація по проєкту ускладнилась.

**MediatR приховує залежності.**
В лабі 2 було явно видно що `AccountsController` залежить від `AccountService`.
Тепер контролер залежить від `IMediator` і не одразу зрозуміло які хендлери він викликає.

**Дублювання структури.**
Кожна операція - окремий record для команди і окремий клас для хендлера.
Для простих операцій як `DepositCommand` це відчувається як надмірність.

## Чим Handler відрізняється від Service

`AccountService` в лабі 2 мав 4 методи і 2 залежності в конструкторі - `IAccountRepository`
і `AccountFactory`. Метод `GetUserAccountsAsync` не потребував `AccountFactory`, але
залежність була присутня. Це порушення ISP і SRP.

Handler має рівно стільки залежностей скільки потрібно для однієї операції:

```csharp
// TransferCommandHandler - тільки репозиторій
public TransferCommandHandler(IAccountRepository accounts)

// GetMyAccountsQueryHandler - тільки read репозиторій
public GetMyAccountsQueryHandler(IAccountReadRepository readRepository)
```

Один Handler = одна операція = одна причина для зміни.

## Як CQS впливає на розширюваність

В лабі 2 щоб додати нову операцію - наприклад зняття коштів - потрібно додати метод
в `AccountService`. Це змінює існуючий клас і потенційно зачіпає інші методи при рефакторингу.

В лабі 3 - створюємо `WithdrawCommand` і `WithdrawCommandHandler`, реєструємо в DI.
Жоден існуючий файл не змінюється. MediatR автоматично знаходить новий хендлер.
Це чистий приклад Open/Closed Principle на практиці.

## Чи відрізняється структура даних Query від доменної моделі

Так, і це принципово важливо. `GetMyAccountsQueryHandler` повертає `AccountReadModel`:

```csharp
public record AccountReadModel(int Id, string AccountNumber, decimal Balance, string Currency);
```

Доменна модель `Account` містить `Currency` як Value Object, має `private set`,
методи `Debit` і `Credit`, захищає інваріанти. Клієнту нічого з цього не потрібно.

Якби Query повертав доменну модель - API зв'язався б з внутрішньою структурою домену.
Перейменували поле в домені — зламався API у клієнтів. З `AccountReadModel` - домен
і API змінюються незалежно.

Також Query читає напряму з БД без маппера:

```csharp
.Select(a => new AccountReadModel(a.Id, a.AccountNumber, a.Balance, a.Currency))
```

Це ефективніше ніж завантажувати доменні об'єкти - менше коду, менше перетворень,
точний SQL запит тільки з потрібними полями.