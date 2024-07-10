using MediatR;
using MoneyControl.Shared;

namespace MoneyControl.Application.Handlers.Transaction.GetTransactionsByPeriod;

public class GetTransactionsByPeriodCommand : IRequest<IEnumerable<TransactionModel>>
{
    public int AccountId { get; set; }
    public DateTime? StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
}