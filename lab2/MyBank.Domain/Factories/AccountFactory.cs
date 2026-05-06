using MyBank.Domain.Errors;
using MyBank.Domain.Models;

namespace MyBank.Domain.Factories;

public class AccountFactory
{
    public (Account? Account, DomainError? Error) Create(int userId, string currency)
    {
        var (currencyVO, error) = Currency.Create(currency);
        if (error != null) return (null, error);

        var account = new Account(
            Guid.NewGuid().ToString("N")[..16].ToUpper(),
            currencyVO!,
            userId);
        return (account, null);
    }
}