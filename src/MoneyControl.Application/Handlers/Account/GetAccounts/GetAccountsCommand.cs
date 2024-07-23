using MediatR;
using MoneyControl.Shared.Models;

namespace MoneyControl.Application.Handlers.Account.GetAccounts;

public class GetAccountsCommand : IRequest<IEnumerable<AccountModel>>
{
    
}