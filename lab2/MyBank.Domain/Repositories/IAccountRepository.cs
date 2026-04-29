using MyBank.Domain.Models;

namespace MyBank.Domain.Repositories;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(int id);
    Task<List<Account>> GetByUserIdAsync(int userId);
    Task AddAsync(Account account);
    Task AddTransactionAsync(Transaction transaction);
    Task SaveChangesAsync();
}