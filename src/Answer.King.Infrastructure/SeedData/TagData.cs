using Answer.King.Domain.Inventory.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Domain.Inventory;
using System.Collections.Generic;
using System;

namespace Answer.King.Infrastructure.SeedData;

public static class TagData
{
    private static readonly TagFactory tagFactory = new();

    public static IList<Tag> Tags { get; } = new List<Tag>
    {
        tagFactory.CreateTag(
            1,
            "Vegan",
            "Non-animal products",
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddHours(-10),
            new List<ProductId>
            {
                new(1)
            },
            false),
        tagFactory.CreateTag(
            2,
            "BOGOF",
            "Buy one get one free!",
            DateTime.UtcNow.AddDays(-2),
            DateTime.UtcNow.AddHours(-30),
            new List<ProductId>
            {
                new(2)
            },
            false)
    };
}
