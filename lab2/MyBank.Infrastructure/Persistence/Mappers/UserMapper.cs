using MyBank.Domain.Models;
using MyBank.Infrastructure.Persistence.Entities;

namespace MyBank.Infrastructure.Persistence.Mappers;

public static class UserMapper
{
    public static User ToDomain(UserEntity entity) =>
        User.Restore(entity.Id, entity.Email, entity.PasswordHash, entity.FullName);

    public static UserEntity ToEntity(User domain) => new()
    {
        Id = domain.Id,
        Email = domain.Email,
        PasswordHash = domain.PasswordHash,
        FullName = domain.FullName
    };
}