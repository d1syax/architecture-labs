namespace MyBank.Domain.Errors;

public class DomainError
{
    public string Code { get; }
    public string Message { get; }

    private DomainError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public static DomainError InvalidEmail() =>
        new("INVALID_EMAIL", "Email is not valid");

    public static DomainError WeakPassword() =>
        new("WEAK_PASSWORD", "Password must be at least 6 characters");

    public static DomainError EmailTaken() =>
        new("EMAIL_TAKEN", "Email is already taken");

    public static DomainError InvalidCredentials() =>
        new("INVALID_CREDENTIALS", "Invalid email or password");

    public static DomainError InvalidCurrency(string currency) =>
        new("INVALID_CURRENCY", $"Currency '{currency}' is not supported");

    public static DomainError AccountNotFound() =>
        new("ACCOUNT_NOT_FOUND", "Account not found");

    public static DomainError InsufficientFunds() =>
        new("INSUFFICIENT_FUNDS", "Insufficient funds for this transfer");

    public static DomainError InvalidAmount() =>
        new("INVALID_AMOUNT", "Amount must be greater than zero");

    public static DomainError SameAccount() =>
        new("SAME_ACCOUNT", "Cannot transfer to the same account");
}