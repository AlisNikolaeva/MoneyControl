using MediatR;

namespace MoneyControl.Shared.Queries.Transaction.UpdateTransaction;

public class UpdateTransactionCommand : IRequest
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public double Sum { get; set; }
    public int CategoryId { get; set; }
    public DateTime DateUtc { get; set; }
}