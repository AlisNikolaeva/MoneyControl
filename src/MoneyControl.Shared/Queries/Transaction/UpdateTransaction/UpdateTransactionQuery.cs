using MediatR;

namespace MoneyControl.Shared.Queries.Transaction.UpdateTransaction;

public class UpdateTransactionQuery : IRequest
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public double Sum { get; set; }
    public DateTime DateUtc { get; set; }
}