using MediatR;
using MoneyControl.Infrastructure;

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
        var account = _dbContext.Accounts.FirstOrDefault(x => x.Id == request.Id);
        if (account == null)
        {
            throw new Exception("Account doesn't exist");
        }
        
        _dbContext.Accounts.Remove(account);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}