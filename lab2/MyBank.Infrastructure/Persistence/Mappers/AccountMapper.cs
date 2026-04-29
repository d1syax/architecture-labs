using MyBank.Domain.Models;
using MyBank.Infrastructure.Persistence.Entities;

namespace MyBank.Infrastructure.Persistence.Mappers;

public static class AccountMapper
{
    public static Account ToDomain(AccountEntity entity)
    {
        var (account, _) = Account.Create(entity.UserId, entity.Currency);
        return account!;
    }

    public static AccountEntity ToEntity(Account domain) => new()
    {
        AccountNumber = domain.AccountNumber,
        Balance = domain.Balance,
        Currency = domain.Currency,
        UserId = domain.UserId
    };
}