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

    public AuthService(IUserRepository users, ITokenService tokens, UserFactory userFactory, IPasswordHasher hasher)
    {
        _users = users;
        _tokens = tokens;
        _userFactory = userFactory;
        _hasher = hasher;
    }

    public async Task<(string? Token, DomainError? Error)> RegisterAsync(string email, string password, string fullName)
    {
        var (user, error) = await _userFactory.CreateAsync(email, password, fullName);
        if (error != null) return (null, error);

        await _users.AddAsync(user!);
        await _users.SaveChangesAsync();
        return (_tokens.Generate(user!), null);
    }

    public async Task<(string? Token, DomainError? Error)> LoginAsync(string email, string password)
    {
        var user = await _users.GetByEmailAsync(email);
        if (user == null || !_hasher.Verify(password, user.PasswordHash))
            return (null, DomainError.InvalidCredentials());

        return (_tokens.Generate(user), null);
    }
}