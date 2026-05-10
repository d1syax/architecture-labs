using MyBank.Application.Auth;
using MyBank.Application.Auth.Commands;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace MyBank.Tests.UnitTests.Commands;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly ITokenService _tokens = Substitute.For<ITokenService>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_users, _tokens, _hasher);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        var emailResult = Email.Create("test@gmail.com");
        var user = User.Restore(1, emailResult.Value, "hashed", "John Doe");
        _users.GetByEmailAsync("test@gmail.com").Returns(user);
        _hasher.Verify("password123", "hashed").Returns(true);
        _tokens.Generate(user).Returns("jwt-token");

        var result = await _handler.Handle(
            new LoginCommand("test@gmail.com", "password123"), default);

        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value);
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsError()
    {
        var emailResult = Email.Create("test@gmail.com");
        var user = User.Restore(1, emailResult.Value, "hashed", "John Doe");
        _users.GetByEmailAsync("test@gmail.com").Returns(user);
        _hasher.Verify("wrongpass", "hashed").Returns(false);

        var result = await _handler.Handle(
            new LoginCommand("test@gmail.com", "wrongpass"), default);

        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_CREDENTIALS", result.Error.Code);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsError()
    {
        _users.GetByEmailAsync("notexist@gmail.com").Returns((User?)null);

        var result = await _handler.Handle(
            new LoginCommand("notexist@gmail.com", "password123"), default);

        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_CREDENTIALS", result.Error.Code);
    }
}