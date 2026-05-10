using CSharpFunctionalExtensions;
using MyBank.Domain.Errors;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;

namespace MyBank.Domain.Factories;

public class UserFactory
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public UserFactory(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<Result<User, DomainError>> CreateAsync(
        string email, string password, string fullName)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure<User, DomainError>(emailResult.Error);

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return Result.Failure<User, DomainError>(DomainError.WeakPassword());

        if (string.IsNullOrWhiteSpace(fullName))
            return Result.Failure<User, DomainError>(DomainError.InvalidFullName());

        if (await _users.ExistsByEmailAsync(emailResult.Value.Value))
            return Result.Failure<User, DomainError>(DomainError.EmailTaken());

        var passwordHash = _hasher.Hash(password);
        var user = new User(emailResult.Value, passwordHash, fullName);
        return Result.Success<User, DomainError>(user);
    }
}