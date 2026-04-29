using MyBank.Application.Auth;
using MyBank.Domain.Factories;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace MyBank.Tests.UnitTests;

public class AuthServiceTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly ITokenService _tokens = Substitute.For<ITokenService>();
    private readonly UserFactory _factory;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _factory = new UserFactory(_users);
        _service = new AuthService(_users, _tokens, _factory);
    }

    [Fact]
    public async Task Register_ValidData_ReturnsToken()
    {
        _users.ExistsByEmailAsync("test@gmail.com").Returns(false);
        _tokens.Generate(Arg.Any<User>()).Returns("jwt-token");

        var (token, error) = await _service.RegisterAsync(
            "test@gmail.com", "password123", "John Doe");

        Assert.Null(error);
        Assert.Equal("jwt-token", token);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsError()
    {
        _users.ExistsByEmailAsync("test@gmail.com").Returns(true);

        var (token, error) = await _service.RegisterAsync(
            "test@gmail.com", "password123", "John Doe");

        Assert.Null(token);
        Assert.Equal("EMAIL_TAKEN", error!.Code);
    }

    [Fact]
    public async Task Register_InvalidEmail_ReturnsError()
    {
        var (token, error) = await _service.RegisterAsync(
            "not-an-email", "password123", "John Doe");

        Assert.Null(token);
        Assert.Equal("INVALID_EMAIL", error!.Code);
    }

    [Fact]
    public async Task Register_WeakPassword_ReturnsError()
    {
        var (token, error) = await _service.RegisterAsync(
            "test@gmail.com", "123", "John Doe");

        Assert.Null(token);
        Assert.Equal("WEAK_PASSWORD", error!.Code);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsError()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("correctpass");
        var user = new User("test@gmail.com", hash, "John Doe");
        _users.GetByEmailAsync("test@gmail.com").Returns(user);

        var (token, error) = await _service.LoginAsync(
            "test@gmail.com", "wrongpass");

        Assert.Null(token);
        Assert.Equal("INVALID_CREDENTIALS", error!.Code);
    }
}