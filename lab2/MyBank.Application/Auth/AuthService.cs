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

    public AuthService(IUserRepository users, ITokenService tokens, UserFactory userFactory)
    {
        _users = users;
        _tokens = tokens;
        _userFactory = userFactory;
    }

    public async Task<(string? Token, DomainError? Error)> RegisterAsync(string email, string password, string fullName)
    {
        var (user, error) = await _userFactory.CreateAsync(email, password, fullName);
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