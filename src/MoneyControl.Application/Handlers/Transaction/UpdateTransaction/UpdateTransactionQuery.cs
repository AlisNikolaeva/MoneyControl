using MediatR;

namespace MoneyControl.Application.Handlers.Transaction.UpdateTransaction;

public class UpdateTransactionQuery : IRequest
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public double Sum { get; set; }
    public DateTime DateUtc { get; set; }
}