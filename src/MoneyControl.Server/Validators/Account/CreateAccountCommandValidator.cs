using FluentValidation;
using MoneyControl.Shared.Queries.Account.CreateAccount;

namespace MoneyControl.Server.Validators.Account;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name must not be empty")
            .MaximumLength(512);
        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency must not be empty")
            .MaximumLength(10);
    }
}