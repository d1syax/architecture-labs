using MyBank.Domain.Models;
using MyBank.Infrastructure.Persistence.Entities;

namespace MyBank.Infrastructure.Persistence.Mappers;

public static class AccountMapper
{
    public static Account ToDomain(AccountEntity entity)
    {
        var currencyResult = Currency.Create(entity.Currency);
        return Account.Restore(entity.Id, entity.AccountNumber,
            entity.Balance, currencyResult.Value, entity.UserId);
    }

    public static AccountEntity ToEntity(Account domain) => new()
    {
        Id = domain.Id,
        AccountNumber = domain.AccountNumber,
        Balance = domain.Balance,
        Currency = domain.Currency.Value,
        UserId = domain.UserId
    };
}