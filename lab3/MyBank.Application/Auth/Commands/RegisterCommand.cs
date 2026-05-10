using CSharpFunctionalExtensions;
using MediatR;
using MyBank.Domain.Errors;

namespace MyBank.Application.Auth.Commands;

public record RegisterCommand(string Email, string Password, string FullName) : IRequest<Result<string, DomainError>>;