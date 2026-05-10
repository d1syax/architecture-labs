using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyBank.Api.DTOs;
using MyBank.Application.Auth.Commands;

namespace MyBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _mediator.Send(
            new RegisterCommand(request.Email, request.Password, request.FullName));

        if (result.IsFailure)
            return result.Error.Code == "EMAIL_TAKEN"
                ? Conflict(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return StatusCode(201, new TokenResponse(result.Value));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(
            new LoginCommand(request.Email, request.Password));

        if (result.IsFailure)
            return Unauthorized(new { error = result.Error.Message });

        return Ok(new TokenResponse(result.Value));
    }
}