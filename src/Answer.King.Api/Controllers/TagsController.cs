using Answer.King.Api.RequestModels;
using Answer.King.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Answer.King.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class TagsController : ControllerBase
{
    public TagsController(ITagService tags, IProductService products)
    {
        this.Tags = tags;
        this.Products = products;
    }

    private ITagService Tags { get; }

    private IProductService Products { get; }

    /// <summary>
    /// Get all tags.
    /// </summary>
    /// <response code="200">When all the tags have been returned.</response>
    /// <returns>All Tags.</returns>
    // GET api/tags
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Domain.Inventory.Tag>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        return this.Ok(await this.Tags.GetTags());
    }

    /// <summary>
    /// Get a single tag.
    /// </summary>
    /// <param name="id">Tag identifier.</param>
    /// <response code="200">When the tag with the provided <paramref name="id"/> has been found.</response>
    /// <response code="404">When the tag with the given <paramref name="id"/> does not exist.</response>
    /// <returns>Tag if found.</returns>
    // GET api/tags/{ID}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Domain.Inventory.Tag), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOne(long id)
    {
        var tag = await this.Tags.GetTag(id);
        if (tag == null)
        {
            return this.NotFound();
        }

        return this.Ok(tag);
    }

    /// <summary>
    /// Create a new tag.
    /// </summary>
    /// <param name="createTag">Tag to create.</param>
    /// <response code="201">When the tag has been created.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <returns>Created Tag.</returns>
    // POST api/tags
    [HttpPost]
    [ProducesResponseType(typeof(Domain.Inventory.Tag), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] Tag createTag)
    {
        var namedTag = await this.Tags.GetTagByName(createTag.Name);

        if (namedTag != null)
        {
            this.ModelState.AddModelError("tag", "A tag with this name already exists");
            return this.BadRequest();
        }

        try
        {
            var tag = await this.Tags.CreateTag(createTag);

            return this.CreatedAtAction(nameof(this.GetOne), new { tag.Id }, tag);
        }
        catch (TagServiceException ex)
        {
            this.ModelState.AddModelError("products", ex.Message);
            return this.ValidationProblem();
        }
    }

    /// <summary>
    /// Update an existing tag.
    /// </summary>
    /// <param name="id">Tag identifier.</param>
    /// <param name="updateTag">Tag details.</param>
    /// <response code="200">When the tag has been updated.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <response code="404">When the tag with the given <paramref name="id"/> does not exist.</response>
    /// <returns>Updated Tag.</returns>
    // PUT api/tags/{ID}
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Domain.Inventory.Tag), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(long id, [FromBody] Tag updateTag)
    {
        var namedTag = await this.Tags.GetTagByName(updateTag.Name);

        if (namedTag != null && id != namedTag.Id)
        {
            this.ModelState.AddModelError("tag", "A tag with this name already exists");
            return this.BadRequest();
        }

        try
        {
            var tag = await this.Tags.UpdateTag(id, updateTag);
            if (tag == null)
            {
                return this.NotFound();
            }

            return this.Ok(tag);
        }
        catch (TagServiceException ex)
        {
            this.ModelState.AddModelError("products", ex.Message);
            return this.ValidationProblem();
        }
    }

    /// <summary>
    /// Retire an existing tag.
    /// </summary>
    /// <param name="id">Tag identifier.</param>
    /// <response code="204">When the tag has been retired.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <response code="404">When the tag with the given <paramref name="id"/> does not exist.</response>
    /// <response code="410">When the tag with the given <paramref name="id"/> is already retired.</response>
    /// <returns>Status of retirement request.</returns>
    // DELETE api/tags/{ID}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> Retire(long id)
    {
        try
        {
            var tag = await this.Tags.RetireTag(id);
            if (tag == null)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
        catch (TagServiceException ex) when (ex.Message.StartsWith(
                                                      "Cannot retire tag whilst there are still products assigned.",
                                                      StringComparison.OrdinalIgnoreCase))
        {
            this.ModelState.AddModelError("products", ex.Message);
            return this.ValidationProblem();
        }
        catch (TagServiceException ex) when (ex.Message.StartsWith(
                                                      "The tag is already retired.", StringComparison.OrdinalIgnoreCase))
        {
            return this.Problem(
                statusCode: StatusCodes.Status410Gone,
                title: "Gone",
                type: "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.9");
        }
    }
}
