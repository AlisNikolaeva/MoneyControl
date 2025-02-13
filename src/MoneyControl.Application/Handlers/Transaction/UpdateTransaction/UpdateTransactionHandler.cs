using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Queries.Transaction.UpdateTransaction;

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
        var transaction = _dbContext.Transactions
            .Include(transactionEntity => transactionEntity.Account)
            .Include(transactionEntity => transactionEntity.Category)
            .FirstOrDefault(x => x.Account.UserId == UserContext.UserId && x.Id == request.Id);
        
        if (transaction == null)
        {
            throw new ValidationException("Transaction doesn't exist",
                [new ValidationFailure("Id", "Transaction doesn't exist")]);
        }

        var newAccount = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken);
        if (newAccount == null)
        {
            throw new ValidationException("Account doesn't exist",
                [new ValidationFailure("AccountId", "Account doesn't exist")]);
        }

        if (transaction.Account.Id != newAccount.Id)
        {
            transaction.Account.Balance -= transaction.Sum;
            transaction.Account = newAccount;
            newAccount.Balance += request.Sum;
        }
        else
        {
            transaction.Account.Balance -= transaction.Sum;
            transaction.Account.Balance += request.Sum;
        }

        CategoryEntity category = null;
        if (request.CategoryId != 0)
        {
            category = _dbContext.Categories.FirstOrDefault(x => x.UserId == UserContext.UserId && x.Id == request.CategoryId);
        }

        transaction.Sum = request.Sum;
        transaction.DateUtc = request.DateUtc;
        transaction.Category = category;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}