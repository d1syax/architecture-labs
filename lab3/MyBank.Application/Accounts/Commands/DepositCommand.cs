using CSharpFunctionalExtensions;
using MediatR;
using MyBank.Domain.Errors;

namespace MyBank.Application.Accounts.Commands;

public record DepositCommand(int UserId, int AccountId, decimal Amount)
    : IRequest<UnitResult<DomainError>>;