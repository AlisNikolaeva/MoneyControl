using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Queries.Transaction.CreateTransaction;

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
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.UserId == UserContext.UserId
                                                                         && x.Id == request.AccountId,
            cancellationToken);

        if (account == null)
        {
            throw new ValidationException("Account doesn't exist.",
                [new ValidationFailure("AccountId", "Account doesn't exist.")]);
        }

        CategoryEntity category = null;
        if (request.CategoryId != 0)
        {
            category = _dbContext.Categories.FirstOrDefault(x => x.UserId == UserContext.UserId && x.Id == request.CategoryId);
            if (category == null)
            {
                throw new ValidationException("Category doesn't exist.",
                    [new ValidationFailure("CategoryId", "Category doesn't exist.")]);
            }
        }

        var transaction = new TransactionEntity
        {
            Account = account,
            Sum = request.Sum,
            Category = category,
            DateUtc = request.DateUtc
        };

        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
        account.Balance += transaction.Sum;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}