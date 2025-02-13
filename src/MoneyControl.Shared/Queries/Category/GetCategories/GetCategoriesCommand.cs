using MediatR;
using MoneyControl.Shared.Models;

namespace MoneyControl.Shared.Queries.Category.GetCategories;

public class GetCategoriesCommand : IRequest<IEnumerable<CategoryModel>>
{
}