using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Queries.Category.DeleteCategory;

namespace MoneyControl.Application.Handlers.Category.DeleteCategory;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteCategoryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = _dbContext.Categories.FirstOrDefault(x => x.UserId == UserContext.UserId && x.Id == request.Id);
        if (category == null)
        {
            throw new ValidationException("Category doesn't exist",
                [new ValidationFailure("Id", "Category doesn't exist")]);
        }

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}