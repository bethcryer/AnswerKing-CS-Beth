using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using Product = Answer.King.Domain.Repositories.Models.Product;

namespace Answer.King.Api.Services.Extensions;

public static class OrderExtensions
{
    public static void AddOrRemoveLineItems(this Order order, RequestModels.OrderDto orderChanges, IList<Product> domainProducts)
    {
        var actions = new List<AddRemoveUpdateAction>();
        actions.AddRange(order.GetLineItemsToAdd(orderChanges));
        actions.AddRange(order.GetLineItemsToRemove(orderChanges));
        actions.AddRange(order.GetLineItemQuantityUpdates(orderChanges));

        foreach (var action in actions)
        {
            if (action.IsIncrease)
            {
                var product = domainProducts.Single(p => p.Id == action.ProductId);
                var categories = product.Categories.Select(c => new Category(c.Id, c.Name, c.Description)).ToList();

                order.AddLineItem(
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Price,
                    categories,
                    action.QuantityDifference);
            }
            else
            {
                order.RemoveLineItem(action.ProductId, action.QuantityDifference);
            }
        }
    }

    private static IEnumerable<AddRemoveUpdateAction> GetLineItemsToRemove(this Order order, RequestModels.OrderDto orderChanges)
    {
        var newProductIds = orderChanges.LineItems.Select(li => li.Product.Id);

        var lineItemsToRemove =
            order.LineItems
                .Where(x
                    => !newProductIds.Contains(x.Product.Id)
                       || orderChanges.LineItems.Single(l => l.Product.Id == x.Product.Id).Quantity == 0)
                .ToList();

        var removeActions =
            lineItemsToRemove.Select(lineItem =>
                new AddRemoveUpdateAction
                {
                    ProductId = lineItem.Product.Id,
                    QuantityDifference = lineItem.Quantity,
                    IsIncrease = false
                });

        return removeActions;
    }

    private static IEnumerable<AddRemoveUpdateAction> GetLineItemsToAdd(this Order order, RequestModels.OrderDto orderChanges)
    {
        var oldProductIds = order.LineItems.Select(li => li.Product.Id);

        var lineItemsToAdd =
            orderChanges.LineItems
                .Where(x => !oldProductIds.Contains(x.Product.Id) && x.Quantity > 0)
                .ToList();

        var addActions =
            lineItemsToAdd.Select(lineItem =>
                new AddRemoveUpdateAction
                {
                    ProductId = lineItem.Product.Id,
                    QuantityDifference = lineItem.Quantity,
                    IsIncrease = true
                });

        return addActions;
    }

    private static IEnumerable<AddRemoveUpdateAction> GetLineItemQuantityUpdates(this Order order, RequestModels.OrderDto orderChanges)
    {
        var newProductIds =
            orderChanges.LineItems
                .Where(li => li.Quantity > 0)
                .Select(li => li.Product.Id);

        var lineItemsToUpdate =
            order.LineItems
                .Where(lineItem => newProductIds.Contains(lineItem.Product.Id))
                .ToList();

        var quantityAddRemoveActions =
            lineItemsToUpdate.Select(lineItem =>
            {
                var updatedLineItem = orderChanges.LineItems.First(li => li.Product.Id == lineItem.Product.Id);

                return new AddRemoveUpdateAction
                {
                    ProductId = lineItem.Product.Id,
                    QuantityDifference = Math.Abs(lineItem.Quantity - updatedLineItem.Quantity),
                    IsIncrease = lineItem.Quantity < updatedLineItem.Quantity
                };
            });

        return quantityAddRemoveActions.Where(action => action.QuantityDifference > 0).ToList();
    }

    private class AddRemoveUpdateAction
    {
        /// <summary>
        /// Product Id to provide when calling <see cref="Order.AddLineItem"/> or <see cref="Order.RemoveLineItem"/>.
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Quantity to add/remove when calling <see cref="Order.AddLineItem"/> or <see cref="Order.RemoveLineItem"/>.
        /// </summary>
        public int QuantityDifference { get; set; }

        /// <summary>
        /// <para>
        /// If set to <see langword="true"/> the value of <see cref="QuantityDifference"/> will be added as the quantity
        /// when calling <see cref="Order.AddLineItem"/>.
        /// </para>
        /// <para>
        /// If set to <see langword="false"/> the value of <see cref="QuantityDifference"/> will be added as the quantity
        /// when calling <see cref="Order.RemoveLineItem"/>.
        /// </para>
        /// </summary>
        public bool IsIncrease { get; set; }
    }
}
