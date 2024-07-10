using MediatR;

namespace MoneyControl.Application.Handlers.Account.DeleteAccount;

public class DeleteAccountCommand : IRequest
{
    public int Id { get; set; }
}