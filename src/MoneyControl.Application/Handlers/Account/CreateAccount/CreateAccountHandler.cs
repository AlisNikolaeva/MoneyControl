using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Queries.Account.CreateAccount;

namespace MoneyControl.Application.Handlers.Account.CreateAccount;

public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, int>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateAccountHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<int> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var exist = await _dbContext.Accounts.AnyAsync(x => x.Name == request.Name, cancellationToken);
        if (exist)
        {
            throw new Exception("Already exists");
        }
        
        var account = new AccountEntity
        {
            Balance = 0,
            Currency = request.Currency,
            Name = request.Name
        };

        await _dbContext.Accounts.AddAsync(account, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return account.Id;
    }
}