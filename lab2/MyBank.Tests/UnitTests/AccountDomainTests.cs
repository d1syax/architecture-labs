using MyBank.Domain.Factories;
using Xunit;

namespace MyBank.Tests.UnitTests;

public class AccountDomainTests
{
    private readonly AccountFactory _factory = new();

    [Fact]
    public void CreateAccount_ValidCurrency_ReturnsAccount()
    {
        var (account, error) = _factory.Create(1, "USD");

        Assert.Null(error);
        Assert.NotNull(account);
        Assert.Equal("USD", account.Currency.Value);
        Assert.Equal(0, account.Balance);
    }

    [Fact]
    public void CreateAccount_InvalidCurrency_ReturnsDomainError()
    {
        var (account, error) = _factory.Create(1, "XYZ");

        Assert.Null(account);
        Assert.NotNull(error);
        Assert.Equal("INVALID_CURRENCY", error!.Code);
    }

    [Fact]
    public void Debit_SufficientFunds_UpdatesBalance()
    {
        var (account, _) = _factory.Create(1, "USD");
        account!.Credit(1000);

        var error = account.Debit(300);

        Assert.Null(error);
        Assert.Equal(700, account.Balance);
    }

    [Fact]
    public void Debit_InsufficientFunds_ReturnsDomainError()
    {
        var (account, _) = _factory.Create(1, "USD");
        account!.Credit(100);

        var error = account.Debit(500);

        Assert.NotNull(error);
        Assert.Equal("INSUFFICIENT_FUNDS", error!.Code);
    }

    [Fact]
    public void Debit_NegativeAmount_ReturnsDomainError()
    {
        var (account, _) = _factory.Create(1, "USD");

        var error = account!.Debit(-100);

        Assert.NotNull(error);
        Assert.Equal("INVALID_AMOUNT", error!.Code);
    }

    [Fact]
    public void Credit_ValidAmount_UpdatesBalance()
    {
        var (account, _) = _factory.Create(1, "USD");

        account!.Credit(500);

        Assert.Equal(500, account.Balance);
    }
}