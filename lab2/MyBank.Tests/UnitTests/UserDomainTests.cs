using MyBank.Domain.Factories;
using MyBank.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace MyBank.Tests.UnitTests;

public class UserDomainTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private UserFactory CreateFactory() => new UserFactory(_users);

    [Fact]
    public async Task CreateUser_ValidData_ReturnsUser()
    {
        _users.ExistsByEmailAsync("test@gmail.com").Returns(false);
        var factory = CreateFactory();

        var (user, error) = await factory.CreateAsync(
            "test@gmail.com", "password123", "John Doe");

        Assert.Null(error);
        Assert.NotNull(user);
        Assert.Equal("test@gmail.com", user.Email);
    }

    [Fact]
    public async Task CreateUser_InvalidEmail_ReturnsDomainError()
    {
        var factory = CreateFactory();

        var (user, error) = await factory.CreateAsync(
            "not-an-email", "password123", "John Doe");

        Assert.Null(user);
        Assert.NotNull(error);
        Assert.Equal("INVALID_EMAIL", error!.Code);
    }

    [Fact]
    public async Task CreateUser_WeakPassword_ReturnsDomainError()
    {
        var factory = CreateFactory();

        var (user, error) = await factory.CreateAsync(
            "test@gmail.com", "123", "John Doe");

        Assert.Null(user);
        Assert.Equal("WEAK_PASSWORD", error!.Code);
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_ReturnsDomainError()
    {
        _users.ExistsByEmailAsync("test@gmail.com").Returns(true);
        var factory = CreateFactory();

        var (user, error) = await factory.CreateAsync(
            "test@gmail.com", "password123", "John Doe");

        Assert.Null(user);
        Assert.Equal("EMAIL_TAKEN", error!.Code);
    }

    [Fact]
    public async Task VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        _users.ExistsByEmailAsync(Arg.Any<string>()).Returns(false);
        var factory = CreateFactory();
        var (user, _) = await factory.CreateAsync(
            "test@gmail.com", "password123", "John Doe");

        Assert.True(user!.VerifyPassword("password123"));
    }

    [Fact]
    public async Task VerifyPassword_WrongPassword_ReturnsFalse()
    {
        _users.ExistsByEmailAsync(Arg.Any<string>()).Returns(false);
        var factory = CreateFactory();
        var (user, _) = await factory.CreateAsync(
            "test@gmail.com", "password123", "John Doe");

        Assert.False(user!.VerifyPassword("wrongpass"));
    }
}