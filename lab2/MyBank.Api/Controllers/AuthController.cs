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
        var (token, error) = await _authService.RegisterAsync(
            request.Email, request.Password, request.FullName);

        if (error != null)
            return error.Code == "EMAIL_TAKEN"
                ? Conflict(new { error = error.Message })
                : BadRequest(new { error = error.Message });

        return StatusCode(201, new TokenResponse(token!));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (token, error) = await _authService.LoginAsync(
            request.Email, request.Password);

        if (error != null) return Unauthorized(new { error = error.Message });
        return Ok(new TokenResponse(token!));
    }
}