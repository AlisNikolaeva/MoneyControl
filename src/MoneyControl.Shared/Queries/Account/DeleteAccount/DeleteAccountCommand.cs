using MediatR;

namespace MoneyControl.Shared.Queries.Account.DeleteAccount;

public class DeleteAccountCommand : IRequest
{
    public int Id { get; set; }
}