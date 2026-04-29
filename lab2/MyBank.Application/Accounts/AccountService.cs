using MyBank.Domain.Errors;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;

namespace MyBank.Application.Accounts;

public class AccountService
{
    private readonly IAccountRepository _accounts;

    public AccountService(IAccountRepository accounts)
    {
        _accounts = accounts;
    }

    public async Task<(Account? Account, DomainError? Error)> CreateAsync(int userId, string currency)
    {
        var (account, error) = Account.Create(userId, currency);
        if (error != null) return (null, error);

        await _accounts.AddAsync(account!);
        await _accounts.SaveChangesAsync();
        return (account, null);
    }

    public async Task<List<Account>> GetUserAccountsAsync(int userId) =>
        await _accounts.GetByUserIdAsync(userId);

    public async Task<DomainError?> TransferAsync(int userId, int fromId, int toId, decimal amount)
    {
        if (fromId == toId) return DomainError.SameAccount();

        var from = await _accounts.GetByIdAsync(fromId);
        if (from == null || from.UserId != userId)
            return DomainError.AccountNotFound();

        var to = await _accounts.GetByIdAsync(toId);
        if (to == null) return DomainError.AccountNotFound();

        var debitError = from.Debit(amount);
        if (debitError != null) return debitError;

        to.Credit(amount);

        await _accounts.AddTransactionAsync(
            Transaction.Create(fromId, toId, amount));
        await _accounts.SaveChangesAsync();
        return null;
    }

    public async Task<DomainError?> DepositAsync(int userId, int accountId, decimal amount)
    {
        var account = await _accounts.GetByIdAsync(accountId);
        if (account == null || account.UserId != userId)
            return DomainError.AccountNotFound();

        var error = account.Credit(amount);
        if (error != null) return error;

        await _accounts.SaveChangesAsync();
        return null;
    }
}