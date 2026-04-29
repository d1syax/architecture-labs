using MyBank.Domain.Errors;

namespace MyBank.Domain.Models;

public class Account
{
    public int Id { get; private set; }
    public string AccountNumber { get; private set; }
    public decimal Balance { get; private set; }
    public string Currency { get; private set; }
    public int UserId { get; private set; }

    private static readonly string[] AllowedCurrencies = ["USD", "UAH", "EUR"];

    private Account(string accountNumber, string currency, int userId)
    {
        AccountNumber = accountNumber;
        Currency = currency;
        UserId = userId;
        Balance = 0;
    }

    public static (Account? Account, DomainError? Error) Create(int userId, string currency)
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

    public DomainError? Debit(decimal amount)
    {
        if (amount <= 0) return DomainError.InvalidAmount();
        if (Balance < amount) return DomainError.InsufficientFunds();
        Balance -= amount;
        return null;
    }

    public DomainError? Credit(decimal amount)
    {
        if (amount <= 0) return DomainError.InvalidAmount();
        Balance += amount;
        return null;
    }
}