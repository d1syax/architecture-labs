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

    internal static User Restore(int id, string email, string passwordHash, string fullName)
    {
        return new User(email, passwordHash, fullName) { Id = id };
    }

    public bool VerifyPassword(string password) =>
        BCrypt.Net.BCrypt.Verify(password, PasswordHash);
}