using Microsoft.AspNetCore.Mvc;
using MyBank.Application.Auth;
using MyBank.Api.DTOs;

namespace MyBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService) => _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(
            request.Email, request.Password, request.FullName);

        if (result.IsFailure)
            return result.Error.Code == "EMAIL_TAKEN"
                ? Conflict(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return StatusCode(201, new TokenResponse(result.Value));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(
            request.Email, request.Password);

        if (result.IsFailure)
            return Unauthorized(new { error = result.Error.Message });

        return Ok(new TokenResponse(result.Value));
    }
}