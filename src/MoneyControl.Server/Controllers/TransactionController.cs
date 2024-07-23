using MediatR;
using Microsoft.AspNetCore.Mvc;
using MoneyControl.Application.Handlers.Transaction.CreateTransaction;
using MoneyControl.Application.Handlers.Transaction.DeleteTransaction;
using MoneyControl.Application.Handlers.Transaction.GetTransactions;
using MoneyControl.Application.Handlers.Transaction.GetTransactionsByPeriod;
using MoneyControl.Application.Handlers.Transaction.UpdateTransaction;
using MoneyControl.Shared;
using MoneyControl.Shared.Models;

namespace MoneyControl.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("create")]
    public async Task<int> Create(CreateTransactionCommand command)
    {
        var id = await _mediator.Send(command);
        return id;
    }
    
    [HttpPost("update")]
    public async Task<IActionResult> Update(UpdateTransactionCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteTransactionCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
    
    [HttpGet]
    public async Task<IEnumerable<TransactionModel>> GetAll()
    {
        var transactions = await _mediator.Send(new GetTransactionsCommand());
        return transactions;
    }
    
    [HttpGet("search")]
    public async Task<IEnumerable<TransactionModel>> Search([FromQuery]SearchTransactionsQuery command)
    {
        var transactions = await _mediator.Send(command);
        return transactions;
    }
}