using MyBank.Domain.Models;
using MyBank.Infrastructure.Persistence.Entities;

namespace MyBank.Infrastructure.Persistence.Mappers;

public static class AccountMapper
{
    public static Account ToDomain(AccountEntity entity) =>
        new Account(entity.AccountNumber, entity.Currency, entity.UserId);

    public static AccountEntity ToEntity(Account domain) => new()
    {
        AccountNumber = domain.AccountNumber,
        Balance = domain.Balance,
        Currency = domain.Currency,
        UserId = domain.UserId
    };
}