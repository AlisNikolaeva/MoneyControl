using MediatR;

namespace MoneyControl.Shared.Queries.Account.CreateAccount;

public class CreateAccountCommand : IRequest<int>
{
    public string Name { get; set; }
    public string Currency { get; set; }
}