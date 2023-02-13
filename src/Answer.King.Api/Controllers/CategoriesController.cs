using Answer.King.Api.RequestModels;
using Answer.King.Api.Services;
using Answer.King.Domain.Repositories.Models;
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
    /// <response code="200">When all the categories have been returned.</response>
    /// <returns>All Categories.</returns>
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
    /// <param name="id">Category identifier.</param>
    /// <response code="200">When the category with the provided <paramref name="id"/> has been found.</response>
    /// <response code="404">When the category with the given <paramref name="id"/> does not exist.</response>
    /// <returns>Category if found.</returns>
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
    /// <param name="createCategory">Category details.</param>
    /// <response code="201">When the category has been created.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <returns>Created Category.</returns>
    // POST api/categories
    [HttpPost]
    [ProducesResponseType(typeof(Domain.Inventory.Category), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Tags = new[] { "Inventory" })]
    public async Task<IActionResult> Post([FromBody] Category createCategory)
    {
        var namedCategory = await this.Categories.GetCategoryByName(createCategory.Name);

        if (namedCategory != null)
        {
            this.ModelState.AddModelError("category", "A category with this name already exists");
            return this.BadRequest();
        }

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
    /// <param name="id">Category identifier.</param>
    /// <param name="updateCategory">Category details.</param>
    /// <response code="200">When the category has been updated.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <response code="404">When the category with the given <paramref name="id"/> does not exist.</response>
    /// <returns>Updated Category.</returns>
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

            var namedCategory = await this.Categories.GetCategoryByName(updateCategory.Name);

            if (namedCategory != null && namedCategory.Id != id)
            {
                this.ModelState.AddModelError("category", "A category with this name already exists");
                return this.BadRequest();
            }

            return this.Ok(category);
        }
        catch (Exception ex) when (ex is CategoryServiceException or ProductLifecycleException)
        {
            this.ModelState.AddModelError("products", ex.Message);
            return this.ValidationProblem();
        }
    }

    /// <summary>
    /// Retire an existing category.
    /// </summary>
    /// <param name="id">Category identifier.</param>
    /// <response code="204">When the category has been retired.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <response code="404">When the category with the given <paramref name="id"/> does not exist.</response>
    /// <response code="410">When the category with the given <paramref name="id"/> is already retired.</response>
    /// <returns>Status of retirement request.</returns>
    // DELETE api/categories/{ID}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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

            return this.NoContent();
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
                statusCode: StatusCodes.Status410Gone,
                title: "Gone",
                type: "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.9");
        }
    }

    /// <summary>
    /// Get all products in a category.
    /// </summary>
    /// <param name="id">Category identifier.</param>
    /// <response code="200">When all the products have been returned.</response>
    /// <response code="404">When the category with the given <paramref name="id"/> does not exist.</response>
    /// <returns>Products associated with provided category identifier.</returns>
    // GET api/categories/{ID}/products
    [HttpGet("{id}/products")]
    [ProducesResponseType(typeof(IEnumerable<Domain.Repositories.Models.Product>), StatusCodes.Status200OK)]
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
