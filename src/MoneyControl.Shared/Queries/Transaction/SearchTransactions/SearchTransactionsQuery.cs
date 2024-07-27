using MediatR;
using MoneyControl.Shared.Models;

namespace MoneyControl.Shared.Queries.Transaction.SearchTransactions;

public class SearchTransactionsQuery : IRequest<IEnumerable<TransactionModel>>
{
    public List<int> AccountIds { get; set; } = new();
    public DateTime? StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
}