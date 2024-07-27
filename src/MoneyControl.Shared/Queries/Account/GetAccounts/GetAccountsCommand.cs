using MediatR;
using MoneyControl.Shared.Models;

namespace MoneyControl.Shared.Queries.Account.GetAccounts;

public class GetAccountsCommand : IRequest<IEnumerable<AccountModel>>
{
    
}