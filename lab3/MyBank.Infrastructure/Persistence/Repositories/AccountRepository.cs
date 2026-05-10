using Microsoft.EntityFrameworkCore;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;
using MyBank.Infrastructure.Persistence.Entities;
using MyBank.Infrastructure.Persistence.Mappers;

namespace MyBank.Infrastructure.Persistence.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly BankDbContext _db;

    public AccountRepository(BankDbContext db) => _db = db;

    public async Task<Account?> GetByIdAsync(int id)
    {
        var entity = await _db.Accounts.FindAsync(id);
        return entity == null ? null : AccountMapper.ToDomain(entity);
    }

    public async Task<List<Account>> GetByUserIdAsync(int userId) =>
        await _db.Accounts
            .Where(a => a.UserId == userId)
            .Select(e => AccountMapper.ToDomain(e))
            .ToListAsync();

    public async Task AddAsync(Account account)
    {
        var entity = AccountMapper.ToEntity(account);
        _db.Accounts.Add(entity);
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        _db.Transactions.Add(new TransactionEntity
        {
            FromAccountId = transaction.FromAccountId,
            ToAccountId = transaction.ToAccountId,
            Amount = transaction.Amount,
            CreatedAt = transaction.CreatedAt
        });
    }

    public async Task SaveChangesAsync() =>
        await _db.SaveChangesAsync();
}