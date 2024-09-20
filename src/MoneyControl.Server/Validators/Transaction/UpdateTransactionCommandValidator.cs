using FluentValidation;
using MoneyControl.Shared.Queries.Transaction.UpdateTransaction;

namespace MoneyControl.Server.Validators.Transaction;

public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommand>
{
    public UpdateTransactionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("AccountId must not be empty");
        RuleFor(x => x.Sum)
            .NotEmpty()
            .NotEqual(0);
        RuleFor(x => x.DateUtc)
            .NotEmpty();
    }
}