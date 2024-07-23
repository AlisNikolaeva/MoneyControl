using MediatR;
using MoneyControl.Shared.Models;

namespace MoneyControl.Application.Handlers.Transaction.GetTransactions;

public class GetTransactionsCommand : IRequest<IEnumerable<TransactionModel>>
{
    
}