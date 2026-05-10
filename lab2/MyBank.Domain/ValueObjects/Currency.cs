using CSharpFunctionalExtensions;
using MyBank.Domain.Errors;

namespace MyBank.Domain.Models;

public class Currency : ValueObject
{
    private static readonly string[] Allowed = ["USD", "UAH", "EUR"];
    public string Value { get; }

    private Currency(string value) => Value = value;

    public static Result<Currency, DomainError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !Allowed.Contains(value.ToUpper()))
            return Result.Failure<Currency, DomainError>(DomainError.InvalidCurrency(value));

        return Result.Success<Currency, DomainError>(new Currency(value.ToUpper()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}