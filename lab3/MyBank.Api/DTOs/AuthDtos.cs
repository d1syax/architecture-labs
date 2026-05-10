namespace MyBank.Api.DTOs;

public record RegisterRequest(string Email, string Password, string FullName);
public record LoginRequest(string Email, string Password);
public record TokenResponse(string Token);