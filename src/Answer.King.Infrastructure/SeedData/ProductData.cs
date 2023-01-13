using System;
using System.Collections.Generic;
using System.Linq;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;

namespace Answer.King.Infrastructure.SeedData;

internal static class ProductData
{
    private static readonly ProductFactory ProductFactory = new();

    public static IList<Product> Products { get; } = new List<Product>
    {
        ProductFactory.CreateProduct(
            1,
            "Fish",
            "Delicious and satisfying.",
            5.99,
            Categories(1),
            Tags(1),
            false),
        ProductFactory.CreateProduct(
            2,
            "Chips",
            "Nothing more to say.",
            2.99,
            Categories(2),
            Tags(2),
            false),
        ProductFactory.CreateProduct(
            3,
            "Gravy",
            "Side",
            0.99,
            Categories(2),
            Array.Empty<TagId>(),
            true),
    };

    private static IList<CategoryId> Categories(long id)
    {
        return CategoryData.Categories.Where(c => c.Id == id)
            .Select(x => new CategoryId(x.Id)).ToList();
    }

    private static IList<TagId> Tags(long id)
    {
        return TagData.Tags.Where(c => c.Id == id)
            .Select(x => new TagId(x.Id)).ToList();
    }
}
