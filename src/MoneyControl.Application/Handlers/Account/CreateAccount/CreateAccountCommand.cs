using MediatR;

namespace MoneyControl.Application.Handlers.Account.CreateAccount;

public class CreateAccountCommand : IRequest<int>
{
    public string Name { get; set; }
    public string Currency { get; set; }
}