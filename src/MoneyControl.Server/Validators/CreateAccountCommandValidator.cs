using FluentValidation;
using MoneyControl.Application.Handlers.Account.CreateAccount;
using MoneyControl.Shared.Queries.Account.CreateAccount;

namespace MoneyControl.Server.Validators;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .MaximumLength(512);
        RuleFor(x => x.Currency)
            .NotNull()
            .MaximumLength(10);
    }
}