using MediatR;
using Microsoft.AspNetCore.Mvc;
using MoneyControl.Application.Handlers.Account.CreateAccount;
using MoneyControl.Application.Handlers.Account.DeleteAccount;
using MoneyControl.Application.Handlers.Account.GetAccounts;
using MoneyControl.Application.Handlers.Account.UpdateAccount;
using MoneyControl.Shared.Models;

namespace MoneyControl.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<object> Create(CreateAccountCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var id = await _mediator.Send(command);
        return id;
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update(UpdateAccountCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _mediator.Send(command);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteAccountCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet]
    public async Task<IEnumerable<AccountModel>> GetAll()
    {
        var accounts = await _mediator.Send(new GetAccountsCommand());
        return accounts;
    }
}