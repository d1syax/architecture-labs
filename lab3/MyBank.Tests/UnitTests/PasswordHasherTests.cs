using MyBank.Infrastructure.Auth;
using Xunit;

namespace MyBank.Tests.UnitTests;

public class PasswordHasherTests
{
    private readonly BcryptPasswordHasher _hasher = new();

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        var hash = _hasher.Hash("password123");
        Assert.True(_hasher.Verify("password123", hash));
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        var hash = _hasher.Hash("password123");
        Assert.False(_hasher.Verify("wrongpass", hash));
    }
}