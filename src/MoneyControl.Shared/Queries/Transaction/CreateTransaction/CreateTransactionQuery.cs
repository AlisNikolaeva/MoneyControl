using MediatR;

namespace MoneyControl.Shared.Queries.Transaction.CreateTransaction;

public class CreateTransactionQuery : IRequest<int>
{
    public string AccountName { get; set; }
    public double Sum { get; set; }
    public DateTime DateUtc { get; set; } = DateTime.UtcNow;
}