using MyBank.Domain.Factories;
using Xunit;

namespace MyBank.Tests.UnitTests;

public class AccountDomainTests
{
    private readonly AccountFactory _factory = new();

    [Fact]
    public void CreateAccount_ValidCurrency_ReturnsAccount()
    {
        var result = _factory.Create(1, "USD");

        Assert.True(result.IsSuccess);
        Assert.Equal("USD", result.Value.Currency.Value);
        Assert.Equal(0, result.Value.Balance);
    }

    [Fact]
    public void CreateAccount_InvalidCurrency_ReturnsDomainError()
    {
        var result = _factory.Create(1, "XYZ");

        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_CURRENCY", result.Error.Code);
    }

    [Fact]
    public void Debit_SufficientFunds_UpdatesBalance()
    {
        var account = _factory.Create(1, "USD").Value;
        account.Credit(1000);

        var result = account.Debit(300);

        Assert.True(result.IsSuccess);
        Assert.Equal(700, account.Balance);
    }

    [Fact]
    public void Debit_InsufficientFunds_ReturnsDomainError()
    {
        var account = _factory.Create(1, "USD").Value;
        account.Credit(100);

        var result = account.Debit(500);

        Assert.True(result.IsFailure);
        Assert.Equal("INSUFFICIENT_FUNDS", result.Error.Code);
    }

    [Fact]
    public void Debit_NegativeAmount_ReturnsDomainError()
    {
        var account = _factory.Create(1, "USD").Value;

        var result = account.Debit(-100);

        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_AMOUNT", result.Error.Code);
    }

    [Fact]
    public void Credit_ValidAmount_UpdatesBalance()
    {
        var account = _factory.Create(1, "USD").Value;

        account.Credit(500);

        Assert.Equal(500, account.Balance);
    }
}