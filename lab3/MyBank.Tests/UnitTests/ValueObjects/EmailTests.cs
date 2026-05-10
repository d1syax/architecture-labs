using MyBank.Domain.Models;
using Xunit;

namespace MyBank.Tests.UnitTests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_ValidEmail_ReturnsEmail()
    {
        var result = Email.Create("test@gmail.com");
        Assert.True(result.IsSuccess);
        Assert.Equal("test@gmail.com", result.Value.Value);
    }

    [Fact]
    public void Create_InvalidEmail_ReturnsError()
    {
        var result = Email.Create("not-an-email");
        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_EMAIL", result.Error.Code);
    }

    [Fact]
    public void Create_EmptyEmail_ReturnsError()
    {
        var result = Email.Create("");
        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_EMAIL", result.Error.Code);
    }

    [Fact]
    public void Create_UpperCaseEmail_Normalizes()
    {
        var result = Email.Create("TEST@GMAIL.COM");
        Assert.True(result.IsSuccess);
        Assert.Equal("test@gmail.com", result.Value.Value);
    }

    [Fact]
    public void Create_EmailWithSpaces_Normalizes()
    {
        var result = Email.Create("  test@gmail.com  ");
        Assert.True(result.IsSuccess);
        Assert.Equal("test@gmail.com", result.Value.Value);
    }

    [Fact]
    public void Equality_SameEmail_AreEqual()
    {
        var email1 = Email.Create("test@gmail.com").Value;
        var email2 = Email.Create("test@gmail.com").Value;
        Assert.Equal(email1, email2);
    }

    [Fact]
    public void Equality_DifferentEmail_AreNotEqual()
    {
        var email1 = Email.Create("test1@gmail.com").Value;
        var email2 = Email.Create("test2@gmail.com").Value;
        Assert.NotEqual(email1, email2);
    }
}