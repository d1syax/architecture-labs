using CSharpFunctionalExtensions;
using MyBank.Domain.Errors;
using MyBank.Domain.Factories;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;

namespace MyBank.Application.Auth;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;
    private readonly UserFactory _userFactory;
    private readonly IPasswordHasher _hasher;

    public AuthService(IUserRepository users, ITokenService tokens,
        UserFactory userFactory, IPasswordHasher hasher)
    {
        _users = users;
        _tokens = tokens;
        _userFactory = userFactory;
        _hasher = hasher;
    }

    public async Task<Result<string, DomainError>> RegisterAsync(
        string email, string password, string fullName)
    {
        var result = await _userFactory.CreateAsync(email, password, fullName);
        if (result.IsFailure)
            return Result.Failure<string, DomainError>(result.Error);

        await _users.AddAsync(result.Value);
        await _users.SaveChangesAsync();
        return Result.Success<string, DomainError>(_tokens.Generate(result.Value));
    }

    public async Task<Result<string, DomainError>> LoginAsync(
        string email, string password)
    {
        var user = await _users.GetByEmailAsync(email);
        if (user == null || !_hasher.Verify(password, user.PasswordHash))
            return Result.Failure<string, DomainError>(DomainError.InvalidCredentials());

        return Result.Success<string, DomainError>(_tokens.Generate(user));
    }
}