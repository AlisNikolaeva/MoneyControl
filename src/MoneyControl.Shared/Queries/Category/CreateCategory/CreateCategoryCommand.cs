using MediatR;

namespace MoneyControl.Shared.Queries.Category.CreateCategory;

public class CreateCategoryCommand : IRequest<int>
{
    public string Name { get; set; }
}