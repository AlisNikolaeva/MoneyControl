using MediatR;
using MoneyControl.Infrastructure;

namespace MoneyControl.Application.Handlers.Account.UpdateAccount;

public class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand>
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateAccountHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = _dbContext.Accounts.FirstOrDefault(x => x.Id == request.Id);
        if (account == null)
        {
            throw new Exception("Account doesn't exist");
        }
        
        account.Name = request.Name;
        account.Currency = request.Currency;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}