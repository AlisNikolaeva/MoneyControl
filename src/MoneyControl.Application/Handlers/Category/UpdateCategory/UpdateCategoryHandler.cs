using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Queries.Category.UpdateCategory;

namespace MoneyControl.Application.Handlers.Category.UpdateCategory;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateCategoryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categories = await _dbContext.Categories.Where(x => x.UserId == UserContext.UserId)
            .ToListAsync(cancellationToken);

        var category = categories.FirstOrDefault(x => x.Id == request.Id);
        if (category == null)
        {
            throw new ValidationException("Category doesn't exist",
                [new ValidationFailure("Id", "Category doesn't exist")]);
        }

        var exist = categories.Any(x => x.Name == request.Name);
        if (exist)
        {
            throw new ValidationException("This category name already exists.",
                [new ValidationFailure("Name", "This category name already exists.")]);
        }

        category.Name = request.Name;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}