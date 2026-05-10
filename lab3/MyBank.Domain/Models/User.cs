using CSharpFunctionalExtensions;

namespace MyBank.Domain.Models;

public class User : Entity<int>
{
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FullName { get; private set; }

    internal User(Email email, string passwordHash, string fullName)
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
    }

    internal static User Restore(int id, Email email,
        string passwordHash, string fullName)
    {
        return new User(email, passwordHash, fullName) { Id = id };
    }
}