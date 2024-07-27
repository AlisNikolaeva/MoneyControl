using MediatR;

namespace MoneyControl.Shared.Queries.Account.UpdateAccount;

public class UpdateAccountCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Currency { get; set; }
}