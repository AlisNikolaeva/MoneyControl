using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;

namespace MoneyControl.Application.Handlers.Transaction.CreateTransaction;

public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, int>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateTransactionHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<int> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken);
        if (account == null)
        {
            throw new Exception("Account doesn't exist");
        }

        var transaction = new TransactionEntity
        {
            Account = account,
            Sum = request.Sum,
            DateUtc = request.DateUtc
        };

        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return transaction.Id;
    }
}