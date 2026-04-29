using MyBank.Domain.Errors;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;

namespace MyBank.Application.Auth;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;

    public AuthService(IUserRepository users, ITokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    public async Task<(string? Token, DomainError? Error)> RegisterAsync(string email, string password, string fullName)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return (null, DomainError.WeakPassword());

        if (await _users.ExistsByEmailAsync(email))
            return (null, DomainError.EmailTaken());

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var (user, error) = User.Create(email, passwordHash, fullName);
        if (error != null) return (null, error);

        await _users.AddAsync(user!);
        return (_tokens.Generate(user!), null);
    }

    public async Task<(string? Token, DomainError? Error)> LoginAsync(string email, string password)
    {
        var user = await _users.GetByEmailAsync(email);
        if (user == null || !user.VerifyPassword(password))
            return (null, DomainError.InvalidCredentials());

        return (_tokens.Generate(user), null);
    }
}