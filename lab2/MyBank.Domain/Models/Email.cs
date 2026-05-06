using MyBank.Domain.Errors;

namespace MyBank.Domain.Models;

public record Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static (Email? Email, DomainError? Error) Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
            return (null, DomainError.InvalidEmail());

        return (new Email(value.ToLower().Trim()), null);
    }

    public override string ToString() => Value;
}