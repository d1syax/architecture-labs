using MyBank.Domain.Errors;
using MyBank.Domain.Models;

namespace MyBank.Domain.Factories;

public class AccountFactory
{
    private static readonly string[] AllowedCurrencies = ["USD", "UAH", "EUR"];

    public (Account? Account, DomainError? Error) Create(int userId, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency) ||
            !AllowedCurrencies.Contains(currency.ToUpper()))
            return (null, DomainError.InvalidCurrency(currency));

        var account = new Account(
            Guid.NewGuid().ToString("N")[..16].ToUpper(),
            currency.ToUpper(),
            userId);
        return (account, null); 
    }
}