using MyBank.Domain.Models;
using MyBank.Infrastructure.Persistence.Entities;

namespace MyBank.Infrastructure.Persistence.Mappers;

public static class UserMapper
{
    public static User ToDomain(UserEntity entity)
    {
        var emailResult = Email.Create(entity.Email);
        return User.Restore(entity.Id, emailResult.Value,
            entity.PasswordHash, entity.FullName);
    }

    public static UserEntity ToEntity(User domain) => new()
    {
        Id = domain.Id,
        Email = domain.Email.Value,
        PasswordHash = domain.PasswordHash,
        FullName = domain.FullName
    };
}