using FluentValidation;
using MoneyControl.Application.Handlers.Account.UpdateAccount;
using MoneyControl.Shared.Queries.Account.UpdateAccount;

namespace MoneyControl.Server.Validators;

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .MaximumLength(512);
        RuleFor(x => x.Currency)
            .NotNull()
            .MaximumLength(10);
    }
}