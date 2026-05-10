using CSharpFunctionalExtensions;
using MediatR;
using MyBank.Domain.Errors;
using MyBank.Domain.Models;
using MyBank.Domain.Repositories;

namespace MyBank.Application.Accounts.Commands;

public class TransferCommandHandler : IRequestHandler<TransferCommand, UnitResult<DomainError>>
{
    private readonly IAccountRepository _accounts;

    public TransferCommandHandler(IAccountRepository accounts)
    {
        _accounts = accounts;
    }

    public async Task<UnitResult<DomainError>> Handle(TransferCommand command, CancellationToken cancellationToken)
    {
        if (command.FromAccountId == command.ToAccountId)
            return UnitResult.Failure<DomainError>(DomainError.SameAccount());

        var from = await _accounts.GetByIdAsync(command.FromAccountId);
        if (from == null || from.UserId != command.UserId)
            return UnitResult.Failure<DomainError>(DomainError.AccountNotFound());

        var to = await _accounts.GetByIdAsync(command.ToAccountId);
        if (to == null)
            return UnitResult.Failure<DomainError>(DomainError.AccountNotFound());

        var debitResult = from.Debit(command.Amount);
        if (debitResult.IsFailure)
            return UnitResult.Failure<DomainError>(debitResult.Error);

        to.Credit(command.Amount);

        await _accounts.AddTransactionAsync(Transaction.Create(command.FromAccountId, command.ToAccountId, command.Amount));
        await _accounts.SaveChangesAsync();
        return UnitResult.Success<DomainError>();
    }
}