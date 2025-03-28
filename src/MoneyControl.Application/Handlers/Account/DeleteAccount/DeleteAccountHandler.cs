using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Queries.Account.DeleteAccount;

namespace MoneyControl.Application.Handlers.Account.DeleteAccount;

public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand>
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteAccountHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = _dbContext.Accounts.FirstOrDefault(x => x.UserId == UserContext.UserId && x.Id == request.Id);
        if (account == null)
        {
            throw new ValidationException("Account doesn't exist", [new ValidationFailure("Name", "Account doesn't exist")]);
        }

        _dbContext.Accounts.Remove(account);
        
        var transactions = _dbContext.Transactions.Where(x => x.Account.Id == request.Id);
        if (transactions.Any())
        {
            _dbContext.Transactions.RemoveRange(transactions);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}