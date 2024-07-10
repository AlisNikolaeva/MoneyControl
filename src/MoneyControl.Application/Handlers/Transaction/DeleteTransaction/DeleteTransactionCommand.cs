using MediatR;

namespace MoneyControl.Application.Handlers.Transaction.DeleteTransaction;

public class DeleteTransactionCommand : IRequest
{
    public int Id { get; set; }
}