using MyBank.Application.ReadModels;

namespace MyBank.Application.Accounts.Queries;

public interface IAccountReadRepository
{
    Task<List<AccountReadModel>> GetByUserIdAsync(int userId);
}