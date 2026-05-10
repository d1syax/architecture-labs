using CSharpFunctionalExtensions;
using MediatR;
using MyBank.Domain.Errors;
using MyBank.Domain.Factories;
using MyBank.Domain.Repositories;

namespace MyBank.Application.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string, DomainError>>
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;
    private readonly UserFactory _factory;

    public RegisterCommandHandler(IUserRepository users, ITokenService tokens, UserFactory factory)
    {
        _users = users;
        _tokens = tokens;
        _factory = factory;
    }

    public async Task<Result<string, DomainError>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await _factory.CreateAsync(
            command.Email, command.Password, command.FullName);
        if (result.IsFailure)
            return Result.Failure<string, DomainError>(result.Error);

        await _users.AddAsync(result.Value);
        await _users.SaveChangesAsync();
        return Result.Success<string, DomainError>(_tokens.Generate(result.Value));
    }
}