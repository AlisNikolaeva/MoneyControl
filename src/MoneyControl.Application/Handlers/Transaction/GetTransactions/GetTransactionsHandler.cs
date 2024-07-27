using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Models;
using MoneyControl.Shared.Queries.Transaction.GetTransactions;

namespace MoneyControl.Application.Handlers.Transaction.GetTransactions;

public class GetTransactionsHandler : IRequestHandler<GetTransactionsCommand, IEnumerable<TransactionModel>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetTransactionsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<TransactionModel>> Handle(GetTransactionsCommand request, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Transactions
            .Include(transactionEntity => transactionEntity.Account)
            .ToListAsync(cancellationToken);
        var transactions = new List<TransactionModel>();
        
        foreach (var item in entities)
        {
            transactions.Add(new TransactionModel
            {
                Id = item.Id,
                AccountId = item.Account.Id,
                Sum = item.Sum,
                DateUtc = item.DateUtc
            });
        }
        return transactions;
    }
}