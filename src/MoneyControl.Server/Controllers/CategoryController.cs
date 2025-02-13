using MediatR;
using Microsoft.AspNetCore.Mvc;
using MoneyControl.Shared.Models;
using MoneyControl.Shared.Queries.Category.CreateCategory;
using MoneyControl.Shared.Queries.Category.DeleteCategory;
using MoneyControl.Shared.Queries.Category.GetCategories;
using MoneyControl.Shared.Queries.Category.UpdateCategory;

namespace MoneyControl.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<object> Create(CreateCategoryCommand command)
    {
        var id = await _mediator.Send(command);
        return id;
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update(UpdateCategoryCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] DeleteCategoryCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet]
    public async Task<IEnumerable<CategoryModel>> GetAll()
    {
        var categories = await _mediator.Send(new GetCategoriesCommand());
        return categories;
    }
}