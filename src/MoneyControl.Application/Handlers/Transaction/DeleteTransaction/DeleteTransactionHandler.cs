using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Queries.Transaction.DeleteTransaction;

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
        var transaction = await _dbContext.Transactions.Include(transactionEntity => transactionEntity.Account)
            .Where(x => x.Account.UserId == Context.UserContext.UserId)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (transaction == null)
        {
            throw new ValidationException("Transaction doesn't exist", 
                [new ValidationFailure("Id", "Transaction doesn't exist")]);
        }

        transaction.Account.Balance -= transaction.Sum;
        _dbContext.Transactions.Remove(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}