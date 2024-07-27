using MediatR;
using MoneyControl.Shared.Models;

namespace MoneyControl.Shared.Queries.Transaction.GetTransactions;

public class GetTransactionsCommand : IRequest<IEnumerable<TransactionModel>>
{
    
}