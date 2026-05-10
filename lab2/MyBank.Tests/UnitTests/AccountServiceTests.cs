using MyBank.Application.Accounts;
using MyBank.Domain.Factories;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace MyBank.Tests.UnitTests;

public class AccountServiceTests
{
    private readonly IAccountRepository _accounts = Substitute.For<IAccountRepository>();
    private readonly AccountFactory _factory = new();
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _service = new AccountService(_accounts, _factory);
    }

    [Fact]
    public async Task CreateAccount_ValidCurrency_ReturnsAccount()
    {
        var result = await _service.CreateAsync(1, "USD");

        Assert.True(result.IsSuccess);
        Assert.Equal("USD", result.Value.Currency.Value);
    }

    [Fact]
    public async Task CreateAccount_InvalidCurrency_ReturnsError()
    {
        var result = await _service.CreateAsync(1, "XYZ");

        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_CURRENCY", result.Error.Code);
    }

    [Fact]
    public async Task Transfer_AccountNotFound_ReturnsError()
    {
        _accounts.GetByIdAsync(1).Returns((Account?)null);

        var result = await _service.TransferAsync(1, 1, 2, 100);

        Assert.True(result.IsFailure);
        Assert.Equal("ACCOUNT_NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task Transfer_SameAccount_ReturnsError()
    {
        var result = await _service.TransferAsync(1, 1, 1, 100);

        Assert.True(result.IsFailure);
        Assert.Equal("SAME_ACCOUNT", result.Error.Code);
    }

    [Fact]
    public async Task Deposit_AccountNotFound_ReturnsError()
    {
        _accounts.GetByIdAsync(999).Returns((Account?)null);

        var result = await _service.DepositAsync(1, 999, 100);

        Assert.True(result.IsFailure);
        Assert.Equal("ACCOUNT_NOT_FOUND", result.Error.Code);
    }
}