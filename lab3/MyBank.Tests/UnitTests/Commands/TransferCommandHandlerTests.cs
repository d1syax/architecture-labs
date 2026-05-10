using MyBank.Application.Accounts.Commands;
using MyBank.Domain.Factories;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace MyBank.Tests.UnitTests.Commands;

public class TransferCommandHandlerTests
{
    private readonly IAccountRepository _accounts = Substitute.For<IAccountRepository>();
    private readonly AccountFactory _factory = new();
    private readonly TransferCommandHandler _handler;

    public TransferCommandHandlerTests()
    {
        _handler = new TransferCommandHandler(_accounts);
    }

    [Fact]
    public async Task Handle_SameAccount_ReturnsError()
    {
        var command = new TransferCommand(1, 1, 1, 100);

        var result = await _handler.Handle(command, default);

        Assert.True(result.IsFailure);
        Assert.Equal("SAME_ACCOUNT", result.Error.Code);
    }

    [Fact]
    public async Task Handle_AccountNotFound_ReturnsError()
    {
        _accounts.GetByIdAsync(1).Returns((Account?)null);
        var command = new TransferCommand(1, 1, 2, 100);

        var result = await _handler.Handle(command, default);

        Assert.True(result.IsFailure);
        Assert.Equal("ACCOUNT_NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task Handle_InsufficientFunds_ReturnsError()
    {
        var from = _factory.Create(1, "USD").Value;
        from.Credit(50);
        var to = _factory.Create(2, "USD").Value;

        _accounts.GetByIdAsync(1).Returns(from);
        _accounts.GetByIdAsync(2).Returns(to);

        var command = new TransferCommand(1, 1, 2, 100);

        var result = await _handler.Handle(command, default);

        Assert.True(result.IsFailure);
        Assert.Equal("INSUFFICIENT_FUNDS", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ValidTransfer_Succeeds()
    {
        var from = _factory.Create(1, "USD").Value;
        from.Credit(500);
        var to = _factory.Create(2, "USD").Value;

        _accounts.GetByIdAsync(1).Returns(from);
        _accounts.GetByIdAsync(2).Returns(to);

        var command = new TransferCommand(1, 1, 2, 200);

        var result = await _handler.Handle(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(300, from.Balance);
        Assert.Equal(200, to.Balance);
    }
}