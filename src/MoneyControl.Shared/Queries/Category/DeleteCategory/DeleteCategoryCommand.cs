using MediatR;

namespace MoneyControl.Shared.Queries.Category.DeleteCategory;

public class DeleteCategoryCommand : IRequest
{
    public int Id { get; set; }
}