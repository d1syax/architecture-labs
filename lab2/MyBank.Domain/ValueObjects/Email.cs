using CSharpFunctionalExtensions;
using MyBank.Domain.Errors;

namespace MyBank.Domain.Models;

public class Email : ValueObject
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email, DomainError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
            return Result.Failure<Email, DomainError>(DomainError.InvalidEmail());

        return Result.Success<Email, DomainError>(new Email(value.ToLower().Trim()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    public override string ToString() => Value;
}