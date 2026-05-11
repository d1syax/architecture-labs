using CSharpFunctionalExtensions;
using MyBank.Domain.Errors;
using MyBank.Domain.Models;

namespace MyBank.Domain.Factories;

public class AccountFactory
{
    public Result<Account, DomainError> Create(int userId, string currency)
    {
        var currencyResult = Currency.Create(currency);
        if (currencyResult.IsFailure)
            return Result.Failure<Account, DomainError>(currencyResult.Error);

        var account = new Account(Guid.NewGuid().ToString("N")[..16].ToUpper(), currencyResult.Value, userId);

        return Result.Success<Account, DomainError>(account);
    }
}