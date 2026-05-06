using MyBank.Domain.Factories;
using MyBank.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace MyBank.Tests.UnitTests;

public class UserDomainTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private UserFactory CreateFactory() => new UserFactory(_users, _hasher);

    [Fact]
    public async Task CreateUser_ValidData_ReturnsUser()
    {
        _users.ExistsByEmailAsync("test@gmail.com").Returns(false);
        var factory = CreateFactory();

        var (user, error) = await factory.CreateAsync(
            "test@gmail.com", "password123", "John Doe");

        Assert.Null(error);
        Assert.NotNull(user);
        Assert.Equal("test@gmail.com", user.Email.Value);
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
}