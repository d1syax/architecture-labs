using CSharpFunctionalExtensions;
using MediatR;
using MyBank.Domain.Errors;
using MyBank.Domain.Repositories;

namespace MyBank.Application.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string, DomainError>>
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;
    private readonly IPasswordHasher _hasher;

    public LoginCommandHandler(IUserRepository users, ITokenService tokens, IPasswordHasher hasher)
    {
        _users = users;
        _tokens = tokens;
        _hasher = hasher;
    }

    public async Task<Result<string, DomainError>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _users.GetByEmailAsync(command.Email);
        if (user == null || !_hasher.Verify(command.Password, user.PasswordHash))
            return Result.Failure<string, DomainError>(DomainError.InvalidCredentials());

        return Result.Success<string, DomainError>(_tokens.Generate(user));
    }
}