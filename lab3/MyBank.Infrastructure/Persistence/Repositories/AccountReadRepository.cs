using Microsoft.EntityFrameworkCore;
using MyBank.Application.Accounts.Queries;
using MyBank.Application.ReadModels;

namespace MyBank.Infrastructure.Persistence.Repositories;

public class AccountReadRepository : IAccountReadRepository
{
    private readonly BankDbContext _db;

    public AccountReadRepository(BankDbContext db) => _db = db;

    public async Task<List<AccountReadModel>> GetByUserIdAsync(int userId) =>
        await _db.Accounts
            .Where(a => a.UserId == userId)
            .Select(a => new AccountReadModel(
                a.Id,
                a.AccountNumber,
                a.Balance,
                a.Currency))
            .ToListAsync();
}