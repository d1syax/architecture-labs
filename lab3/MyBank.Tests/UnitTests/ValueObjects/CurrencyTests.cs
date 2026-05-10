using MyBank.Domain.Models;
using Xunit;

namespace MyBank.Tests.UnitTests.ValueObjects;

public class CurrencyTests
{
    [Fact]
    public void Create_USD_ReturnsSuccess()
    {
        var result = Currency.Create("USD");
        Assert.True(result.IsSuccess);
        Assert.Equal("USD", result.Value.Value);
    }

    [Fact]
    public void Create_UAH_ReturnsSuccess()
    {
        var result = Currency.Create("UAH");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_EUR_ReturnsSuccess()
    {
        var result = Currency.Create("EUR");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_LowerCase_Normalizes()
    {
        var result = Currency.Create("usd");
        Assert.True(result.IsSuccess);
        Assert.Equal("USD", result.Value.Value);
    }

    [Fact]
    public void Create_Unknown_ReturnsError()
    {
        var result = Currency.Create("GBP");
        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_CURRENCY", result.Error.Code);
    }

    [Fact]
    public void Create_Empty_ReturnsError()
    {
        var result = Currency.Create("");
        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_CURRENCY", result.Error.Code);
    }

    [Fact]
    public void Equality_SameCurrency_AreEqual()
    {
        var c1 = Currency.Create("USD").Value;
        var c2 = Currency.Create("USD").Value;
        Assert.Equal(c1, c2);
    }

    [Fact]
    public void Equality_DifferentCurrency_AreNotEqual()
    {
        var c1 = Currency.Create("USD").Value;
        var c2 = Currency.Create("EUR").Value;
        Assert.NotEqual(c1, c2);
    }
}