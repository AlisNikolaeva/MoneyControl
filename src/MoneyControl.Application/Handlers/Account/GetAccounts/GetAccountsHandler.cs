using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Models;

namespace MoneyControl.Application.Handlers.Account.GetAccounts;

public class GetAccountsHandler : IRequestHandler<GetAccountsCommand, IEnumerable<AccountModel>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetAccountsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<AccountModel>> Handle(GetAccountsCommand request, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Accounts.ToListAsync(cancellationToken);
        var accounts = new List<AccountModel>();
        foreach (var item in entities)
        {
            accounts.Add(new AccountModel
            {
                Id = item.Id,
                Balance = item.Balance,
                Currency = item.Currency,
                Name = item.Name
            });
        }
        return accounts;
    }
}