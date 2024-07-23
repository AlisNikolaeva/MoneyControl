using MediatR;

namespace MoneyControl.Application.Handlers.Transaction.DeleteTransaction;

public class DeleteTransactionQuery : IRequest
{
    public int Id { get; set; }
}