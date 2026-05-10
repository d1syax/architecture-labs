using MyBank.Application.Auth;
using MyBank.Application.Auth.Commands;
using MyBank.Domain.Factories;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace MyBank.Tests.UnitTests.Commands;

public class RegisterCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly ITokenService _tokens = Substitute.For<ITokenService>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        var factory = new UserFactory(_users, _hasher);
        _handler = new RegisterCommandHandler(_users, _tokens, factory);
    }

    [Fact]
    public async Task Handle_ValidData_ReturnsToken()
    {
        _users.ExistsByEmailAsync("test@gmail.com").Returns(false);
        _tokens.Generate(Arg.Any<User>()).Returns("jwt-token");

        var result = await _handler.Handle(
            new RegisterCommand("test@gmail.com", "password123", "John Doe"), default);

        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsError()
    {
        _users.ExistsByEmailAsync("test@gmail.com").Returns(true);

        var result = await _handler.Handle(
            new RegisterCommand("test@gmail.com", "password123", "John Doe"), default);

        Assert.True(result.IsFailure);
        Assert.Equal("EMAIL_TAKEN", result.Error.Code);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ReturnsError()
    {
        var result = await _handler.Handle(
            new RegisterCommand("not-an-email", "password123", "John Doe"), default);

        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_EMAIL", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WeakPassword_ReturnsError()
    {
        var result = await _handler.Handle(
            new RegisterCommand("test@gmail.com", "123", "John Doe"), default);

        Assert.True(result.IsFailure);
        Assert.Equal("WEAK_PASSWORD", result.Error.Code);
    }
}