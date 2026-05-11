using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBank.Application.Accounts;
using MyBank.Api.DTOs;

namespace MyBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountsController(AccountService accountService) =>
        _accountService = accountService;

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var result = await _accountService.CreateAsync(GetUserId(), request.Currency);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return StatusCode(201, new AccountResponse(
            result.Value.Id,
            result.Value.AccountNumber,
            result.Value.Balance,
            result.Value.Currency.Value));
    }

    [HttpGet]
    public async Task<IActionResult> GetMyAccounts()
    {
        var accounts = await _accountService.GetUserAccountsAsync(GetUserId());
        return Ok(accounts.Select(a =>
            new AccountResponse(a.Id, a.AccountNumber, a.Balance, a.Currency.Value)));
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        var result = await _accountService.TransferAsync(
            GetUserId(), request.FromAccountId, request.ToAccountId, request.Amount);

        if (result.IsFailure)
            return result.Error.Code == "ACCOUNT_NOT_FOUND"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return Ok(new { message = "Transfer successful" });
    }

    [HttpPost("{accountId}/deposit")]
    public async Task<IActionResult> Deposit(int accountId, [FromBody] DepositRequest request)
    {
        var result = await _accountService.DepositAsync(
            GetUserId(), accountId, request.Amount);

        if (result.IsFailure)
            return result.Error.Code == "ACCOUNT_NOT_FOUND"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return Ok(new { message = "Deposit successful" });
    }
}