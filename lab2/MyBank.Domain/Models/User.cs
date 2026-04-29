using MyBank.Domain.Errors;

namespace MyBank.Domain.Models;

public class User
{
    public int Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FullName { get; private set; }
    private readonly List<Account> _accounts = new();
    public IReadOnlyCollection<Account> Accounts => _accounts.AsReadOnly();

    private User(string email, string passwordHash, string fullName)
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
    }

    public static (User? User, DomainError? Error) Create(string email, string passwordHash, string fullName)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return (null, DomainError.InvalidEmail());

        if (string.IsNullOrWhiteSpace(fullName))
            return (null, DomainError.InvalidEmail());

        return (new User(email, passwordHash, fullName), null);
    }

    public bool VerifyPassword(string password) =>
        BCrypt.Net.BCrypt.Verify(password, PasswordHash);
}