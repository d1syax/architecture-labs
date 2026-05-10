using CSharpFunctionalExtensions;
using MediatR;
using MyBank.Domain.Errors;

namespace MyBank.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<string, DomainError>>;