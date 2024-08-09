using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Models;
using MoneyControl.Shared.Queries.Transaction.SearchTransactions;

namespace MoneyControl.Application.Handlers.Transaction.SearchTransactions;

public class SearchTransactionsHandler : IRequestHandler<SearchTransactionsQuery, IEnumerable<TransactionModel>>
{
    private readonly ApplicationDbContext _dbContext;

    public SearchTransactionsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TransactionModel>> Handle(SearchTransactionsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Transactions.Where(x => request.AccountIds.Contains(x.Account.Id));
        if (request.StartUtc.HasValue)
        {
            query = query.Where(x => x.DateUtc >= request.StartUtc.Value);
        }
        
        if (request.EndUtc.HasValue)
        {
            query = query.Where(x => x.DateUtc <= request.EndUtc.Value);
        }
        
        var filteredTransactions = await query.Select(x => new TransactionModel
        {
            AccountId = x.Account.Id,
            AccountName = x.Account.Name,
            DateUtc = x.DateUtc,
            Id = x.Id,
            Sum = x.Sum
        }).ToListAsync(cancellationToken);

        return filteredTransactions;
    }
}