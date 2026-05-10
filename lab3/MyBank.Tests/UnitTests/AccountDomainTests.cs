using MyBank.Domain.Factories;
using MyBank.Domain.Models;
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
    public void CreateAccount_UAH_ReturnsAccount()
    {
        var result = _factory.Create(1, "UAH");
        Assert.True(result.IsSuccess);
        Assert.Equal("UAH", result.Value.Currency.Value);
    }

    [Fact]
    public void CreateAccount_EUR_ReturnsAccount()
    {
        var result = _factory.Create(1, "EUR");
        Assert.True(result.IsSuccess);
        Assert.Equal("EUR", result.Value.Currency.Value);
    }

    [Fact]
    public void CreateAccount_LowerCaseCurrency_Normalizes()
    {
        var result = _factory.Create(1, "usd");
        Assert.True(result.IsSuccess);
        Assert.Equal("USD", result.Value.Currency.Value);
    }

    [Fact]
    public void CreateAccount_EmptyCurrency_ReturnsError()
    {
        var result = _factory.Create(1, "");
        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_CURRENCY", result.Error.Code);
    }

    [Fact]
    public void CreateAccount_InitialBalance_IsZero()
    {
        var result = _factory.Create(1, "USD");
        Assert.Equal(0, result.Value.Balance);
    }

    [Fact]
    public void CreateAccount_HasAccountNumber()
    {
        var result = _factory.Create(1, "USD");
        Assert.NotEmpty(result.Value.AccountNumber);
        Assert.Equal(16, result.Value.AccountNumber.Length);
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
    public void Debit_ZeroAmount_ReturnsDomainError()
    {
        var account = _factory.Create(1, "USD").Value;
        var result = account.Debit(0);
        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_AMOUNT", result.Error.Code);
    }

    [Fact]
    public void Debit_ExactBalance_Succeeds()
    {
        var account = _factory.Create(1, "USD").Value;
        account.Credit(100);
        var result = account.Debit(100);
        Assert.True(result.IsSuccess);
        Assert.Equal(0, account.Balance);
    }

    [Fact]
    public void Credit_ValidAmount_UpdatesBalance()
    {
        var account = _factory.Create(1, "USD").Value;
        account.Credit(500);
        Assert.Equal(500, account.Balance);
    }

    [Fact]
    public void Credit_NegativeAmount_ReturnsDomainError()
    {
        var account = _factory.Create(1, "USD").Value;
        var result = account.Credit(-100);
        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_AMOUNT", result.Error.Code);
    }

    [Fact]
    public void Credit_ZeroAmount_ReturnsDomainError()
    {
        var account = _factory.Create(1, "USD").Value;
        var result = account.Credit(0);
        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_AMOUNT", result.Error.Code);
    }

    [Fact]
    public void Credit_MultipleCredits_AccumulatesBalance()
    {
        var account = _factory.Create(1, "USD").Value;
        account.Credit(100);
        account.Credit(200);
        account.Credit(300);
        Assert.Equal(600, account.Balance);
    }

    [Fact]
    public void Debit_AfterMultipleCredits_UpdatesCorrectly()
    {
        var account = _factory.Create(1, "USD").Value;
        account.Credit(500);
        account.Credit(300);
        var result = account.Debit(200);
        Assert.True(result.IsSuccess);
        Assert.Equal(600, account.Balance);
    }
}