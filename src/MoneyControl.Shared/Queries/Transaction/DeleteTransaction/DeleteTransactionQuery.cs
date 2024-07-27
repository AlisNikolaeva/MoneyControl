using MediatR;

namespace MoneyControl.Shared.Queries.Transaction.DeleteTransaction;

public class DeleteTransactionQuery : IRequest
{
    public int Id { get; set; }
}