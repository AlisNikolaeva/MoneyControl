using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using MoneyControl.Shared;

namespace MoneyControl.Application.Handlers.Transaction.GetTransactionsByPeriod;

public class GetTransactionsByPeriodHandler : IRequestHandler<GetTransactionsByPeriodCommand, IEnumerable<TransactionModel>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetTransactionsByPeriodHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TransactionModel>> Handle(GetTransactionsByPeriodCommand request, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken);
        if (account == null)
        {
            throw new Exception("Account doesn't exist");
        }

        var query = _dbContext.Transactions.Where(x => x.Account.Id == request.AccountId);
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
            DateUtc = x.DateUtc,
            Id = x.Id,
            Sum = x.Sum
        }).ToListAsync(cancellationToken);

        return filteredTransactions;
    }
}