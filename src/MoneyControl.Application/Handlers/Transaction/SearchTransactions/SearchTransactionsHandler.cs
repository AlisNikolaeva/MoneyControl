using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Models;
using MoneyControl.Shared.Queries.Transaction.SearchTransactions;

namespace MoneyControl.Application.Handlers.Transaction.SearchTransactions;

public class SearchTransactionsHandler : IRequestHandler<SearchTransactionsQuery, TransactionsModel>
{
    private readonly ApplicationDbContext _dbContext;

    public SearchTransactionsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TransactionsModel> Handle(SearchTransactionsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Transactions.Where(x => request.AccountIds.Contains(x.Account.Id))
            .Where(x => x.Account.UserId == UserContext.UserId);
        if (request.StartUtc.HasValue)
        {
            query = query.Where(x => x.DateUtc >= request.StartUtc.Value);
        }
        
        if (request.EndUtc.HasValue)
        {
            query = query.Where(x => x.DateUtc <= request.EndUtc.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = query.OrderByDescending(x => x.DateUtc)
            .ThenBy(x => x.Account.Name)
            .Skip(request.Offset)
            .Take(request.Count);
        
        var filteredTransactions = await query.Select(x => new TransactionModel
        {
            AccountId = x.Account.Id,
            AccountName = x.Account.Name,
            DateUtc = x.DateUtc,
            Id = x.Id,
            Sum = x.Sum
        }).ToListAsync(cancellationToken);

        var result = new TransactionsModel
        {
            Items = filteredTransactions,
            TotalCount = totalCount
        };

        return result;
    }
}