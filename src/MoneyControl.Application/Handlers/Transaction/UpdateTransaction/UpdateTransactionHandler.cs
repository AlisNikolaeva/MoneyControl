using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Infrastructure;

namespace MoneyControl.Application.Handlers.Transaction.UpdateTransaction;

public class UpdateTransactionHandler : IRequestHandler<UpdateTransactionCommand>
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateTransactionHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken);
        if (account == null)
        {
            throw new Exception("Account doesn't exist");
        }
        
        var transaction = _dbContext.Transactions.FirstOrDefault(x => x.Id == request.Id);
        if (transaction == null)
        {
            throw new Exception("Transaction doesn't exist");
        }

        transaction.Account = account;
        transaction.Sum = request.Sum;
        transaction.DateUtc = request.DateUtc;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}