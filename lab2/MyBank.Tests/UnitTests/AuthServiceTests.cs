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
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly UserFactory _factory;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _factory = new UserFactory(_users, _hasher);
        _service = new AuthService(_users, _tokens, _factory, _hasher);
    }

    [Fact]
    public async Task Register_ValidData_ReturnsToken()
    {
        _users.ExistsByEmailAsync("test@gmail.com").Returns(false);
        _tokens.Generate(Arg.Any<User>()).Returns("jwt-token");

        var result = await _service.RegisterAsync(
            "test@gmail.com", "password123", "John Doe");

        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsError()
    {
        _users.ExistsByEmailAsync("test@gmail.com").Returns(true);

        var result = await _service.RegisterAsync(
            "test@gmail.com", "password123", "John Doe");

        Assert.True(result.IsFailure);
        Assert.Equal("EMAIL_TAKEN", result.Error.Code);
    }

    [Fact]
    public async Task Register_InvalidEmail_ReturnsError()
    {
        var result = await _service.RegisterAsync(
            "not-an-email", "password123", "John Doe");

        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_EMAIL", result.Error.Code);
    }

    [Fact]
    public async Task Register_WeakPassword_ReturnsError()
    {
        var result = await _service.RegisterAsync(
            "test@gmail.com", "123", "John Doe");

        Assert.True(result.IsFailure);
        Assert.Equal("WEAK_PASSWORD", result.Error.Code);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsError()
    {
        var emailResult = Email.Create("test@gmail.com");
        var user = User.Restore(1, emailResult.Value, "hashed", "John Doe");
        _users.GetByEmailAsync("test@gmail.com").Returns(user);
        _hasher.Verify("wrongpass", "hashed").Returns(false);

        var result = await _service.LoginAsync("test@gmail.com", "wrongpass");

        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_CREDENTIALS", result.Error.Code);
    }
}