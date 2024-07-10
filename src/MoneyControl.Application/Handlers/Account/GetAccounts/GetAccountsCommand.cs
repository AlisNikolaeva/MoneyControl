using MediatR;
using MoneyControl.Shared;

namespace MoneyControl.Application.Handlers.Account.GetAccounts;

public class GetAccountsCommand : IRequest<IEnumerable<AccountModel>>
{
    
}