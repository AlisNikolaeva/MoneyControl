using MediatR;
using MoneyControl.Shared;
using MoneyControl.Shared.Models;

namespace MoneyControl.Application.Handlers.Transaction.GetTransactionsByPeriod;

public class SearchTransactionsQuery : IRequest<IEnumerable<TransactionModel>>
{
    public List<int> AccountIds { get; set; } = new();
    public DateTime? StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
}