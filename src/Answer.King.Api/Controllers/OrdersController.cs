using Answer.King.Api.Services;
using Answer.King.Domain.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Answer.King.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    public OrdersController(IOrderService orders)
    {
        this.Orders = orders;
    }

    private IOrderService Orders { get; }

    /// <summary>
    /// Get all orders.
    /// </summary>
    /// <response code="200">When all the orders have been returned.</response>
    /// <returns>All Orders.</returns>
    // GET api/orders
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        return this.Ok(await this.Orders.GetOrders());
    }

    /// <summary>
    /// Get a single order.
    /// </summary>
    /// <param name="id">Order identifier.</param>
    /// <response code="200">When the order with the provided <paramref name="id"/> has been found.</response>
    /// <response code="404">When the order with the given <paramref name="id"/> does not exist.</response>
    /// <returns>Order if found.</returns>
    // GET api/orders/{ID}
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOne(long id)
    {
        var order = await this.Orders.GetOrder(id);
        if (order == null)
        {
            return this.NotFound();
        }

        return this.Ok(order);
    }

    /// <summary>
    /// Create a new order.
    /// </summary>
    /// <param name="createOrder">Order details.</param>
    /// <response code="201">When the order has been created.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <returns>Created Order.</returns>
    // POST api/orders
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] RequestModels.Order createOrder)
    {
        try
        {
            var order = await this.Orders.CreateOrder(createOrder);

            return this.CreatedAtAction(nameof(this.GetOne), new { order.Id }, order);
        }
        catch (ProductInvalidException ex)
        {
            this.ModelState.AddModelError("LineItems.ProductId", ex.Message);
            return this.ValidationProblem();
        }
    }

    /// <summary>
    /// Update an existing order.
    /// </summary>
    /// <param name="id">Order identifier.</param>
    /// <param name="updateOrder">Order details.</param>
    /// <response code="200">When the order has been updated.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <response code="404">When the order with the given <paramref name="id"/> does not exist.</response>
    /// <returns>Updated Order.</returns>
    // PUT api/orders/{ID}
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(long id, [FromBody] RequestModels.Order updateOrder)
    {
        try
        {
            var order = await this.Orders.UpdateOrder(id, updateOrder);

            if (order == null)
            {
                return this.NotFound();
            }

            return this.Ok(order);
        }
        catch (ProductInvalidException ex)
        {
            this.ModelState.AddModelError("LineItems.ProductId", ex.Message);
            return this.ValidationProblem();
        }
        catch (OrderLifeCycleException ex)
        {
            this.ModelState.AddModelError("order", ex.Message);
            return this.ValidationProblem();
        }
    }

    /// <summary>
    /// Cancel an existing order.
    /// </summary>
    /// <param name="id">Order identifier.</param>
    /// <response code="204">When the order has been cancelled.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <response code="404">When the order with the given <paramref name="id"/> does not exist.</response>
    /// <returns>Status of cancel request.</returns>
    // DELETE api/orders/{ID}
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(long id)
    {
        try
        {
            var order = await this.Orders.CancelOrder(id);
            if (order == null)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
        catch (OrderLifeCycleException ex)
        {
            this.ModelState.AddModelError("order", ex.Message);
            return this.ValidationProblem();
        }
    }
}
