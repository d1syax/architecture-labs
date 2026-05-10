using CSharpFunctionalExtensions;
using MyBank.Domain.Errors;
using MyBank.Domain.Factories;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;

namespace MyBank.Application.Accounts;

public class AccountService
{
    private readonly IAccountRepository _accounts;
    private readonly AccountFactory _accountFactory;

    public AccountService(IAccountRepository accounts, AccountFactory accountFactory)
    {
        _accounts = accounts;
        _accountFactory = accountFactory;
    }

    public async Task<Result<Account, DomainError>> CreateAsync(int userId, string currency)
    {
        var result = _accountFactory.Create(userId, currency);
        if (result.IsFailure)
            return Result.Failure<Account, DomainError>(result.Error);

        await _accounts.AddAsync(result.Value);
        await _accounts.SaveChangesAsync();
        return Result.Success<Account, DomainError>(result.Value);
    }

    public async Task<List<Account>> GetUserAccountsAsync(int userId) =>
        await _accounts.GetByUserIdAsync(userId);

    public async Task<UnitResult<DomainError>> TransferAsync(
        int userId, int fromId, int toId, decimal amount)
    {
        if (fromId == toId)
            return UnitResult.Failure<DomainError>(DomainError.SameAccount());

        var from = await _accounts.GetByIdAsync(fromId);
        if (from == null || from.UserId != userId)
            return UnitResult.Failure<DomainError>(DomainError.AccountNotFound());

        var to = await _accounts.GetByIdAsync(toId);
        if (to == null)
            return UnitResult.Failure<DomainError>(DomainError.AccountNotFound());

        var debitResult = from.Debit(amount);
        if (debitResult.IsFailure)
            return UnitResult.Failure<DomainError>(debitResult.Error);

        to.Credit(amount);

        await _accounts.AddTransactionAsync(
            Transaction.Create(fromId, toId, amount));
        await _accounts.SaveChangesAsync();
        return UnitResult.Success<DomainError>();
    }

    public async Task<UnitResult<DomainError>> DepositAsync(
        int userId, int accountId, decimal amount)
    {
        var account = await _accounts.GetByIdAsync(accountId);
        if (account == null || account.UserId != userId)
            return UnitResult.Failure<DomainError>(DomainError.AccountNotFound());

        var creditResult = account.Credit(amount);
        if (creditResult.IsFailure)
            return UnitResult.Failure<DomainError>(creditResult.Error);

        await _accounts.SaveChangesAsync();
        return UnitResult.Success<DomainError>();
    }
}