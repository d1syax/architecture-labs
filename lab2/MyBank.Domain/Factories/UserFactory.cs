using MyBank.Domain.Errors;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;

namespace MyBank.Domain.Factories;

public class UserFactory
{
    private readonly IUserRepository _users;

    public UserFactory(IUserRepository users)
    {
        _users = users;
    }

    public async Task<(User? User, DomainError? Error)> CreateAsync(string email, string password, string fullName)
    {
        var (emailVO, emailError) = Email.Create(email);
        if (emailError != null) return (null, emailError);
        
        if (await _users.ExistsByEmailAsync(emailVO!.Value))
            return (null, DomainError.EmailTaken());

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return (null, DomainError.WeakPassword());

        if (string.IsNullOrWhiteSpace(fullName))
            return (null, DomainError.InvalidFullName());

        if (await _users.ExistsByEmailAsync(email))
            return (null, DomainError.EmailTaken());

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User(emailVO, passwordHash, fullName);
        return (user, null);
    }
}