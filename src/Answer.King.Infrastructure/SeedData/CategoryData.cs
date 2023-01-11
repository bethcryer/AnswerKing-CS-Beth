using System;
using System.Collections.Generic;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Infrastructure.Repositories.Mappings;

namespace Answer.King.Infrastructure.SeedData;

public static class CategoryData
{
    private static readonly CategoryFactory categoryFactory = new();

    public static IList<Category> Categories { get; } = new List<Category>
    {
        categoryFactory.CreateCategory(
            1,
            "Seafood",
            "Food from the oceans",
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddHours(-10),
            new List<ProductId>
            {
                new(1)
            },
            false),
        categoryFactory.CreateCategory(
            2,
            "Sundries",
            "Things that go with things.",
            DateTime.UtcNow.AddDays(-2),
            DateTime.UtcNow.AddHours(-30),
            new List<ProductId>
            {
                new(2)
            },
            false)
    };
}
