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
        var (account, error) = await _service.CreateAsync(1, "USD");

        Assert.Null(error);
        Assert.NotNull(account);
        Assert.Equal("USD", account.Currency.Value);
    }

    [Fact]
    public async Task CreateAccount_InvalidCurrency_ReturnsError()
    {
        var (account, error) = await _service.CreateAsync(1, "XYZ");

        Assert.Null(account);
        Assert.Equal("INVALID_CURRENCY", error!.Code);
    }

    [Fact]
    public async Task Transfer_AccountNotFound_ReturnsError()
    {
        _accounts.GetByIdAsync(1).Returns((Account?)null);

        var error = await _service.TransferAsync(1, 1, 2, 100);

        Assert.Equal("ACCOUNT_NOT_FOUND", error!.Code);
    }

    [Fact]
    public async Task Transfer_SameAccount_ReturnsError()
    {
        var error = await _service.TransferAsync(1, 1, 1, 100);

        Assert.Equal("SAME_ACCOUNT", error!.Code);
    }
    
    [Fact]
    public async Task Deposit_AccountNotFound_ReturnsError()
    {
        _accounts.GetByIdAsync(999).Returns((Account?)null);

        var error = await _service.DepositAsync(1, 999, 100);

        Assert.Equal("ACCOUNT_NOT_FOUND", error!.Code);
    }
}