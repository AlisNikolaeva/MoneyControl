using MediatR;

namespace MoneyControl.Application.Handlers.DeleteAccount;

public class DeleteAccountCommand : IRequest
{
    public int Id { get; set; }
}