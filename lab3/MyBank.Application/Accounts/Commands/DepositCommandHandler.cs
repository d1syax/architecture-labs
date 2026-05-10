using CSharpFunctionalExtensions;
using MediatR;
using MyBank.Domain.Errors;
using MyBank.Domain.Repositories;

namespace MyBank.Application.Accounts.Commands;

public class DepositCommandHandler : IRequestHandler<DepositCommand, UnitResult<DomainError>>
{
    private readonly IAccountRepository _accounts;

    public DepositCommandHandler(IAccountRepository accounts)
    {
        _accounts = accounts;
    }

    public async Task<UnitResult<DomainError>> Handle(DepositCommand command, CancellationToken cancellationToken)
    {
        var account = await _accounts.GetByIdAsync(command.AccountId);
        if (account == null || account.UserId != command.UserId)
            return UnitResult.Failure<DomainError>(DomainError.AccountNotFound());

        var result = account.Credit(command.Amount);
        if (result.IsFailure)
            return UnitResult.Failure<DomainError>(result.Error);

        await _accounts.SaveChangesAsync();
        return UnitResult.Success<DomainError>();
    }
}