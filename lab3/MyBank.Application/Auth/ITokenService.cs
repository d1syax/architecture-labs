using MyBank.Domain.Models;

namespace MyBank.Application.Auth;

public interface ITokenService
{
    string Generate(User user);
}