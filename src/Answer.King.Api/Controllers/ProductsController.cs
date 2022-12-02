using Answer.King.Api.Services;
using Answer.King.Domain.Repositories.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Answer.King.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    public ProductsController(IProductService products)
    {
        this.Products = products;
    }

    private IProductService Products { get; }

    /// <summary>
    /// Get all products.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">When all the products have been returned.</response>
    // GET api/products
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
    [SwaggerOperation(Tags = new[] { "Inventory" })]
    public async Task<IActionResult> GetAll()
    {
        return this.Ok(await this.Products.GetProducts());
    }

    /// <summary>
    /// Get a single product.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="200">When the product with the provided <paramref name="id"/> has been found.</response>
    /// <response code="404">When the product with the given <paramref name="id"/> does not exist.</response>
    // GET api/products/{ID}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Tags = new[] { "Inventory" })]
    public async Task<IActionResult> GetOne(long id)
    {
        var product = await this.Products.GetProduct(id);

        if (product == null)
        {
            return this.NotFound();
        }

        return this.Ok(product);
    }

    /// <summary>
    /// Create a new product.
    /// </summary>
    /// <param name="createProduct"></param>
    /// <response code="201">When the product has been created.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    // POST api/products
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Tags = new[] { "Inventory" })]
    public async Task<IActionResult> Post([FromBody] RequestModels.Product createProduct)
    {
        try
        {
            var product = await this.Products.CreateProduct(createProduct);

            return this.CreatedAtAction(nameof(this.GetOne), new { product.Id }, product);
        }
        catch (ProductServiceException ex)
        {
            this.ModelState.AddModelError("category", ex.Message);
            return this.ValidationProblem();
        }
    }

    /// <summary>
    /// Update an existing product.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateProduct"></param>
    /// <response code="200">When the product has been updated.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <response code="404">When the product with the given <paramref name="id"/> does not exist.</response>
    // PUT api/products/{ID}
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Tags = new[] { "Inventory" })]
    public async Task<IActionResult> Put(long id, [FromBody] RequestModels.Product updateProduct)
    {
        try
        {
            var product = await this.Products.UpdateProduct(id, updateProduct);

            if (product == null)
            {
                return this.NotFound();
            }

            return this.Ok(product);
        }
        catch (ProductServiceException ex)
        {
            this.ModelState.AddModelError("category", ex.Message);
            return this.ValidationProblem();
        }
    }

    /// <summary>
    /// Retire an existing product.
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">When the product has been retired.</response>
    /// <response code="404">When the product with the given <paramref name="id"/> does not exist.</response>
    /// <response code="410">When the product with the given <paramref name="id"/> is already retired.</response>
    // DELETE api/products/{ID}
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [SwaggerOperation(Tags = new[] { "Inventory" })]
    public async Task<IActionResult> Retire(long id)
    {
        try
        {
            var product = await this.Products.RetireProduct(id);

            if (product == null)
            {
                return this.NotFound();
            }

            return this.Ok(product);
        }
        catch (ProductServiceException)
        {
            return this.Problem(
                statusCode: StatusCodes.Status410Gone,
                title: "Gone",
                type: "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.9");
        }
    }
}
