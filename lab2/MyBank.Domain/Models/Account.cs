using MyBank.Domain.Errors;

namespace MyBank.Domain.Models;

public class Account
{
    public int Id { get; private set; }
    public string AccountNumber { get; private set; }
    public decimal Balance { get; private set; }
    public string Currency { get; private set; }
    public int UserId { get; private set; }

    internal Account(string accountNumber, string currency, int userId)
    {
        AccountNumber = accountNumber;
        Currency = currency;
        UserId = userId;
        Balance = 0;
    }

    internal static Account Restore(int id, string accountNumber, decimal balance, string currency, int userId)
    {
        return new Account(accountNumber, currency, userId)
        {
            Id = id,
            Balance = balance
        };
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