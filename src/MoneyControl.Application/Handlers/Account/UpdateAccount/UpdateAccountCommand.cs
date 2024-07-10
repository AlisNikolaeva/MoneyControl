using MediatR;

namespace MoneyControl.Application.Handlers.Account.UpdateAccount;

public class UpdateAccountCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Currency { get; set; }
}