using MyBank.Domain.Models;
using MyBank.Infrastructure.Persistence.Entities;

namespace MyBank.Infrastructure.Persistence.Mappers;

public static class AccountMapper
{
    public static Account ToDomain(AccountEntity entity) =>
        Account.Restore(entity.Id, entity.AccountNumber, entity.Balance, entity.Currency, entity.UserId);

    public static AccountEntity ToEntity(Account domain) => new()
    {
        Id = domain.Id,
        AccountNumber = domain.AccountNumber,
        Balance = domain.Balance,
        Currency = domain.Currency,
        UserId = domain.UserId
    };
}