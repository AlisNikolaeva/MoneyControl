using MediatR;
using MoneyControl.Shared;

namespace MoneyControl.Application.Handlers.GetAccounts;

public class GetAccountsCommand : IRequest<IEnumerable<AccountModel>>
{
    
}