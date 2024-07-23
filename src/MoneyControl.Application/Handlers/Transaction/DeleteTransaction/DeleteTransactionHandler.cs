using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Infrastructure;

namespace MoneyControl.Application.Handlers.Transaction.DeleteTransaction;

public class DeleteTransactionHandler : IRequestHandler<DeleteTransactionQuery>
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteTransactionHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Handle(DeleteTransactionQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (transaction == null)
        {
            throw new Exception("Transaction doesn't exist");
        }
        
        _dbContext.Transactions.Remove(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}