using MediatR;

namespace MoneyControl.Shared.Queries.Category.UpdateCategory;

public class UpdateCategoryCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
}