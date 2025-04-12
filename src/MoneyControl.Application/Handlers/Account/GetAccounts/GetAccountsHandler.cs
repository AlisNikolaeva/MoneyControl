using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Models;
using MoneyControl.Shared.Queries.Account.GetAccounts;

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
        var accounts = await _dbContext.Accounts.Where(x => x.UserId == UserContext.UserId)
            .OrderByDescending(x => x.CreatedUtc)
            .Select(x => new AccountModel
            {
                Id = x.Id,
                Balance = x.Balance,
                Currency = x.Currency,
                Name = x.Name,
                CreatedUtc = x.CreatedUtc
            })
            .ToListAsync(cancellationToken);

        return accounts;
    }
}