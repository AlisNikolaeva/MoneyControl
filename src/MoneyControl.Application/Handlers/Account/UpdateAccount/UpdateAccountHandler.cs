using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Queries.Account.UpdateAccount;

namespace MoneyControl.Application.Handlers.Account.UpdateAccount;

public class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand>
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateAccountHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = _dbContext.Accounts.FirstOrDefault(x => x.Id == request.Id && x.UserId == Context.UserContext.UserId);
        if (account == null)
        {
            throw new ValidationException("Account doesn't exist", [new ValidationFailure("Name", "Account doesn't exist")]);
        }
        
        var exist = await _dbContext.Accounts.AnyAsync(x => x.Name == request.Name, cancellationToken);
        if (exist)
        {
            throw new ValidationException("This account name already exists.", 
                [new ValidationFailure("Name", "This account name already exists.")]);
        }
        
        account.Name = request.Name;
        account.Currency = request.Currency;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}