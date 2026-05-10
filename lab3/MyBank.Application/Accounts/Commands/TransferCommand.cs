using CSharpFunctionalExtensions;
using MediatR;
using MyBank.Domain.Errors;

namespace MyBank.Application.Accounts.Commands;

public record TransferCommand(int UserId, int FromAccountId, int ToAccountId, decimal Amount) : IRequest<UnitResult<DomainError>>;