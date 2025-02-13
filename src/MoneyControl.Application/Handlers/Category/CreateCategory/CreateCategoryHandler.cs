using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Queries.Category.CreateCategory;

namespace MoneyControl.Application.Handlers.Category.CreateCategory;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, int>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateCategoryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var exist = await _dbContext.Categories.AnyAsync(x => x.UserId == UserContext.UserId && x.Name == request.Name,
            cancellationToken);
        
        if (exist)
        {
            throw new ValidationException("This category name already exists.",
                [new ValidationFailure("Name", "This category name already exists.")]);
        }

        var category = new CategoryEntity
        {
            Name = request.Name,
            UserId = UserContext.UserId
        };

        await _dbContext.Categories.AddAsync(category, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}