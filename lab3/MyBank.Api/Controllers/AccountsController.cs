using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBank.Api.DTOs;
using MyBank.Application.Accounts.Commands;
using MyBank.Application.Accounts.Queries;

namespace MyBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator) => _mediator = mediator;

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var result = await _mediator.Send(
            new CreateAccountCommand(GetUserId(), request.Currency));

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return StatusCode(201, new { id = result.Value });
    }

    [HttpGet]
    public async Task<IActionResult> GetMyAccounts()
    {
        var accounts = await _mediator.Send(new GetMyAccountsQuery(GetUserId()));
        return Ok(accounts);
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        var result = await _mediator.Send(
            new TransferCommand(
                GetUserId(),
                request.FromAccountId,
                request.ToAccountId,
                request.Amount));

        if (result.IsFailure)
            return result.Error.Code == "ACCOUNT_NOT_FOUND"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return Ok(new { message = "Transfer successful" });
    }

    [HttpPost("{accountId}/deposit")]
    public async Task<IActionResult> Deposit(int accountId, [FromBody] DepositRequest request)
    {
        var result = await _mediator.Send(
            new DepositCommand(GetUserId(), accountId, request.Amount));

        if (result.IsFailure)
            return result.Error.Code == "ACCOUNT_NOT_FOUND"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return Ok(new { message = "Deposit successful" });
    }
}