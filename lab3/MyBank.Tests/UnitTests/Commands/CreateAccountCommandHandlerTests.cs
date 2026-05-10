using CSharpFunctionalExtensions;
using MyBank.Application.Accounts.Commands;
using MyBank.Domain.Factories;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace MyBank.Tests.UnitTests.Commands;

public class CreateAccountCommandHandlerTests
{
    private readonly IAccountRepository _accounts = Substitute.For<IAccountRepository>();
    private readonly AccountFactory _factory = new();
    private readonly CreateAccountCommandHandler _handler;

    public CreateAccountCommandHandlerTests()
    {
        _handler = new CreateAccountCommandHandler(_accounts, _factory);
    }

    [Fact]
    public async Task Handle_InvalidCurrency_ReturnsError()
    {
        var command = new CreateAccountCommand(1, "XYZ");

        var result = await _handler.Handle(command, default);

        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_CURRENCY", result.Error.Code);
        await _accounts.DidNotReceive().AddAsync(Arg.Any<Account>());
    }
}