using MyBank.Application.Accounts.Commands;
using MyBank.Domain.Factories;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace MyBank.Tests.UnitTests.Commands;

public class DepositCommandHandlerTests
{
    private readonly IAccountRepository _accounts = Substitute.For<IAccountRepository>();
    private readonly AccountFactory _factory = new();
    private readonly DepositCommandHandler _handler;

    public DepositCommandHandlerTests()
    {
        _handler = new DepositCommandHandler(_accounts);
    }

    [Fact]
    public async Task Handle_ValidDeposit_Succeeds()
    {
        var account = _factory.Create(1, "USD").Value;
        _accounts.GetByIdAsync(1).Returns(account);

        var result = await _handler.Handle(new DepositCommand(1, 1, 500), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(500, account.Balance);
        await _accounts.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_AccountNotFound_ReturnsError()
    {
        _accounts.GetByIdAsync(999).Returns((Account?)null);

        var result = await _handler.Handle(new DepositCommand(1, 999, 100), default);

        Assert.True(result.IsFailure);
        Assert.Equal("ACCOUNT_NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task Handle_AccountBelongsToDifferentUser_ReturnsError()
    {
        var account = _factory.Create(2, "USD").Value;
        _accounts.GetByIdAsync(1).Returns(account);

        var result = await _handler.Handle(new DepositCommand(1, 1, 100), default);

        Assert.True(result.IsFailure);
        Assert.Equal("ACCOUNT_NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task Handle_NegativeAmount_ReturnsError()
    {
        var account = _factory.Create(1, "USD").Value;
        _accounts.GetByIdAsync(1).Returns(account);

        var result = await _handler.Handle(new DepositCommand(1, 1, -100), default);

        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_AMOUNT", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ZeroAmount_ReturnsError()
    {
        var account = _factory.Create(1, "USD").Value;
        _accounts.GetByIdAsync(1).Returns(account);

        var result = await _handler.Handle(new DepositCommand(1, 1, 0), default);

        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_AMOUNT", result.Error.Code);
    }

    [Fact]
    public async Task Handle_MultipleDeposits_AccumulatesBalance()
    {
        var account = _factory.Create(1, "USD").Value;
        _accounts.GetByIdAsync(1).Returns(account);

        await _handler.Handle(new DepositCommand(1, 1, 100), default);
        await _handler.Handle(new DepositCommand(1, 1, 200), default);
        await _handler.Handle(new DepositCommand(1, 1, 300), default);

        Assert.Equal(600, account.Balance);
    }
}