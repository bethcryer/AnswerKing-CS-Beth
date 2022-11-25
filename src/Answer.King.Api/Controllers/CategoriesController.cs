using Answer.King.Api.RequestModels;
using Answer.King.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Answer.King.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    public CategoriesController(ICategoryService categories, IProductService products)
    {
        this.Categories = categories;
        this.Products = products;
    }

    private ICategoryService Categories { get; }

    private IProductService Products { get; }

    /// <summary>
    /// Get all categories.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">When all the categories have been returned.</response>
    // GET api/categories
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Domain.Inventory.Category>), StatusCodes.Status200OK)]
    [SwaggerOperation(Tags = new[] { "Inventory" })]
    public async Task<IActionResult> GetAll()
    {
        return this.Ok(await this.Categories.GetCategories());
    }

    /// <summary>
    /// Get a single category.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="200">When the category with the provided <paramref name="id"/> has been found.</response>
    /// <response code="404">When the category with the given <paramref name="id"/> does not exist</response>
    // GET api/categories/{ID}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Domain.Inventory.Category), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Tags = new[] { "Inventory" })]
    public async Task<IActionResult> GetOne(long id)
    {
        var category = await this.Categories.GetCategory(id);
        if (category == null)
        {
            return this.NotFound();
        }

        return this.Ok(category);
    }

    /// <summary>
    /// Create a new category.
    /// </summary>
    /// <param name="createCategory"></param>
    /// <response code="201">When the category has been created.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    // POST api/categories
    [HttpPost]
    [ProducesResponseType(typeof(Domain.Inventory.Category), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Tags = new[] { "Inventory" })]
    public async Task<IActionResult> Post([FromBody] Category createCategory)
    {
        try
        {
            var category = await this.Categories.CreateCategory(createCategory);

            return this.CreatedAtAction(nameof(this.GetOne), new { category.Id }, category);
        }
        catch (CategoryServiceException ex)
        {
            this.ModelState.AddModelError("products", ex.Message);
            return this.ValidationProblem();
        }
    }

    /// <summary>
    /// Update an existing category.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateCategory"></param>
    /// <response code="200">When the category has been updated.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <response code="404">When the category with the given <paramref name="id"/> does not exist.</response>
    // PUT api/categories/{ID}
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Domain.Inventory.Category), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Tags = new[] { "Inventory" })]
    public async Task<IActionResult> Put(long id, [FromBody] Category updateCategory)
    {
        try
        {
            var category = await this.Categories.UpdateCategory(id, updateCategory);
            if (category == null)
            {
                return this.NotFound();
            }

            return this.Ok(category);
        }
        catch (CategoryServiceException ex)
        {
            this.ModelState.AddModelError("products", ex.Message);
            return this.ValidationProblem();
        }
    }

    /// <summary>
    /// Retire an existing category.
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">When the category has been retired.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <response code="404">When the category with the given <paramref name="id"/> does not exist.</response>
    /// <response code="410">When the category with the given <paramref name="id"/> is already retired.</response>
    // DELETE api/categories/{ID}
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Domain.Inventory.Category), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [SwaggerOperation(Tags = new[] { "Inventory" })]
    public async Task<IActionResult> Retire(long id)
    {
        try
        {
            var category = await this.Categories.RetireCategory(id);

            if (category == null)
            {
                return this.NotFound();
            }

            return this.Ok(category);
        }
        catch (CategoryServiceException ex) when (ex.Message.StartsWith(
                                                      "Cannot retire category whilst there are still products assigned.",
                                                      StringComparison.OrdinalIgnoreCase))
        {
            this.ModelState.AddModelError("products", ex.Message);
            return this.ValidationProblem();
        }
        catch (CategoryServiceException ex) when (ex.Message.StartsWith(
                                                      "The category is already retired.", StringComparison.OrdinalIgnoreCase))
        {
            return this.Problem(
                type: "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.9",
                title: "Gone",
                statusCode: StatusCodes.Status410Gone);
        }
    }

    /// <summary>
    /// Get all products in a category.
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">When all the products have been returned.</response>
    /// <response code="404">When the category with the given <paramref name="id"/> does not exist.</response>
    // GET api/categories/{ID}/products
    [HttpGet("{id}/products")]
    [ProducesResponseType(typeof(IEnumerable<Answer.King.Domain.Repositories.Models.Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Tags = new[] { "Inventory" })]
    public async Task<IActionResult> GetProducts(long id)
    {
        var category = await this.Categories.GetCategory(id);

        if (category == null)
        {
            return this.NotFound();
        }

        var productIds = category.Products.Select(p => p.Value);

        var products = await this.Products.GetProducts(productIds);

        return this.Ok(products);
    }
}
