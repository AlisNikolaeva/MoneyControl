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
        var account = await _dbContext.Accounts.FirstOrDefaultAsync
            (x => x.Id == request.AccountId && x.UserId == UserContext.UserId, cancellationToken);
        if (account == null)
        {
            throw new ValidationException("Account does not exist.", 
                [new ValidationFailure("AccountId", "Account does not exist.")]);
        }

        var transaction = new TransactionEntity
        {
            Account = account,
            Sum = request.Sum,
            DateUtc = request.DateUtc.Date
        };
        
        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);

        account.Balance += transaction.Sum;
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return transaction.Id;
    }
}