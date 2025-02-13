using MediatR;

namespace MoneyControl.Shared.Queries.Transaction.CreateTransaction;

public class CreateTransactionCommand : IRequest<int>
{
    public int AccountId { get; set; }
    public double Sum { get; set; }
    public int CategoryId { get; set; }
    public DateTime DateUtc { get; set; } = DateTime.UtcNow;
}