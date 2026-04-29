using MyBank.Domain.Errors;

namespace MyBank.Domain.Models;

public class User
{
    public int Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FullName { get; private set; }

    internal User(string email, string passwordHash, string fullName)
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
    }

    public bool VerifyPassword(string password) =>
        BCrypt.Net.BCrypt.Verify(password, PasswordHash);
}