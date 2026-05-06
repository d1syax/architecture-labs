using MyBank.Domain.Errors;

namespace MyBank.Domain.Models;

public record Currency
{
    private static readonly string[] Allowed = ["USD", "UAH", "EUR"];
    public string Value { get; }

    private Currency(string value) => Value = value;

    public static (Currency? Currency, DomainError? Error) Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !Allowed.Contains(value.ToUpper()))
            return (null, DomainError.InvalidCurrency(value));

        return (new Currency(value.ToUpper()), null);
    }

    public override string ToString() => Value;
}