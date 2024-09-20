using FluentValidation;
using MoneyControl.Shared.Queries.Transaction.CreateTransaction;

namespace MoneyControl.Server.Validators.Transaction;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("AccountId must not be empty");
        RuleFor(x => x.Sum)
            .NotEmpty()
            .NotEqual(0);
    }
}