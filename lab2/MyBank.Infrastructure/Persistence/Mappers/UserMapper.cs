using MyBank.Domain.Models;
using MyBank.Infrastructure.Persistence.Entities;

namespace MyBank.Infrastructure.Persistence.Mappers;

public static class UserMapper
{
    public static User ToDomain(UserEntity entity) =>
        new User(entity.Email, entity.PasswordHash, entity.FullName);

    public static UserEntity ToEntity(User domain) => new()
    {
        Email = domain.Email,
        PasswordHash = domain.PasswordHash,
        FullName = domain.FullName
    };
}