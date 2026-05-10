using CSharpFunctionalExtensions;
using MediatR;
using MyBank.Domain.Errors;

namespace MyBank.Application.Accounts.Commands;

public record CreateAccountCommand(int UserId, string Currency) : IRequest<Result<int, DomainError>>;