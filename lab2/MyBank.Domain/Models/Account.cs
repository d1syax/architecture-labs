using CSharpFunctionalExtensions;
using MyBank.Domain.Errors;

namespace MyBank.Domain.Models;

public class Account : Entity<int>
{
    public string AccountNumber { get; private set; }
    public decimal Balance { get; private set; }
    public Currency Currency { get; private set; }
    public int UserId { get; private set; }

    internal Account(string accountNumber, Currency currency, int userId)
    {
        AccountNumber = accountNumber;
        Currency = currency;
        UserId = userId;
        Balance = 0;
    }

    internal static Account Restore(int id, string accountNumber, decimal balance, Currency currency, int userId)
    {
        return new Account(accountNumber, currency, userId)
        {
            Id = id,
            Balance = balance
        };
    }

    public UnitResult<DomainError> Debit(decimal amount)
    {
        if (amount <= 0)
            return UnitResult.Failure<DomainError>(DomainError.InvalidAmount());
        if (Balance < amount)
            return UnitResult.Failure<DomainError>(DomainError.InsufficientFunds());

        Balance -= amount;
        return UnitResult.Success<DomainError>();
    }

    public UnitResult<DomainError> Credit(decimal amount)
    {
        if (amount <= 0)
            return UnitResult.Failure<DomainError>(DomainError.InvalidAmount());

        Balance += amount;
        return UnitResult.Success<DomainError>();
    }
}