using MediatR;

namespace MoneyControl.Application.Handlers.Transaction.CreateTransaction;

public class CreateTransactionCommand : IRequest<int>
{
    public int AccountId { get; set; }
    public double Sum { get; set; }
    public DateTime DateUtc { get; set; } = DateTime.UtcNow;
}