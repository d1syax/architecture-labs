using Microsoft.EntityFrameworkCore;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;
using MyBank.Infrastructure.Persistence.Mappers;

namespace MyBank.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BankDbContext _db;

    public UserRepository(BankDbContext db) => _db = db;

    public async Task<User?> GetByEmailAsync(string email)
    {
        var entity = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        return entity == null ? null : UserMapper.ToDomain(entity);
    }

    public async Task<bool> ExistsByEmailAsync(string email) =>
        await _db.Users.AnyAsync(u => u.Email == email);

    public async Task AddAsync(User user)
    {
        var entity = UserMapper.ToEntity(user);
        _db.Users.Add(entity);
        await _db.SaveChangesAsync();
    }
}