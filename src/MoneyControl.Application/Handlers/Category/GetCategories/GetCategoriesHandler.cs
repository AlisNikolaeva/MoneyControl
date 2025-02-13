using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Models;
using MoneyControl.Shared.Queries.Category.GetCategories;

namespace MoneyControl.Application.Handlers.Category.GetCategories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesCommand, IEnumerable<CategoryModel>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetCategoriesHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<CategoryModel>> Handle(GetCategoriesCommand request,
        CancellationToken cancellationToken)
    {
        var categories = await _dbContext.Categories.Where(x => x.UserId == UserContext.UserId)
            .Select(x => new CategoryModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);

        return categories;
    }
}