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
        var (account, error) = await _accountService.CreateAsync(GetUserId(), request.Currency);
        if (error != null) return BadRequest(new { error = error.Message });

        return StatusCode(201, new AccountResponse(
            account!.Id, account.AccountNumber, account.Balance, account.Currency.Value));
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
        var error = await _accountService.TransferAsync(
            GetUserId(), request.FromAccountId, request.ToAccountId, request.Amount);

        if (error != null)
            return error.Code == "ACCOUNT_NOT_FOUND"
                ? NotFound(new { error = error.Message })
                : BadRequest(new { error = error.Message });

        return Ok(new { message = "Transfer successful" });
    }

    [HttpPost("{accountId}/deposit")]
    public async Task<IActionResult> Deposit(int accountId, [FromBody] DepositRequest request)
    {
        var error = await _accountService.DepositAsync(GetUserId(), accountId, request.Amount);

        if (error != null)
            return error.Code == "ACCOUNT_NOT_FOUND"
                ? NotFound(new { error = error.Message })
                : BadRequest(new { error = error.Message });

        return Ok(new { message = "Deposit successful" });
    }
}