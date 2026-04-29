namespace MyBank.Api.DTOs;

public record CreateAccountRequest(string Currency);
public record TransferRequest(int FromAccountId, int ToAccountId, decimal Amount);
public record DepositRequest(decimal Amount);
public record AccountResponse(int Id, string AccountNumber, decimal Balance, string Currency);