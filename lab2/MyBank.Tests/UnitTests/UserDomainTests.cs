using MyBank.Domain.Models;

namespace MyBank.Tests.UnitTests;

public class UserDomainTests
{
    [Fact]
    public void CreateUser_ValidData_ReturnsUser()
    {
        var (user, error) = User.Create("test@gmail.com", "hashedpass", "John Doe");

        Assert.Null(error);
        Assert.NotNull(user);
        Assert.Equal("test@gmail.com", user.Email);
    }

    [Fact]
    public void CreateUser_InvalidEmail_ReturnsDomainError()
    {
        var (user, error) = User.Create("not-an-email", "hashedpass", "John Doe");

        Assert.Null(user);
        Assert.NotNull(error);
        Assert.Equal("INVALID_EMAIL", error.Code);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("password123");
        var (user, _) = User.Create("test@gmail.com", hash, "John Doe");

        Assert.True(user!.VerifyPassword("password123"));
    }

    [Fact]
    public void VerifyPassword_WrongPassword_ReturnsFalse()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("password123");
        var (user, _) = User.Create("test@gmail.com", hash, "John Doe");

        Assert.False(user!.VerifyPassword("wrongpass"));
    }
}