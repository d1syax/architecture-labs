using CSharpFunctionalExtensions;
using MediatR;
using MyBank.Domain.Errors;
using MyBank.Domain.Factories;
using MyBank.Domain.Repositories;

namespace MyBank.Application.Accounts.Commands;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result<int, DomainError>>
{
    private readonly IAccountRepository _accounts;
    private readonly AccountFactory _factory;

    public CreateAccountCommandHandler(IAccountRepository accounts, AccountFactory factory)
    {
        _accounts = accounts;
        _factory = factory;
    }

    public async Task<Result<int, DomainError>> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var result = _factory.Create(command.UserId, command.Currency);
        if (result.IsFailure)
            return Result.Failure<int, DomainError>(result.Error);

        var id = await _accounts.AddAsync(result.Value);
        return Result.Success<int, DomainError>(id);
    }
}