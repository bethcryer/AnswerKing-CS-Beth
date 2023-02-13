using System;
using System.Collections.Generic;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Infrastructure.Repositories.Mappings;

namespace Answer.King.Infrastructure.SeedData;

public static class TagData
{
    private static readonly TagFactory TagFactory = new();

    private static readonly DateTime Now = DateTime.UtcNow;

    public static IList<Tag> Tags { get; } = new List<Tag>
    {
        TagFactory.CreateTag(
            1,
            "Vegan",
            "Non-animal products",
            Now.AddDays(-1),
            Now.AddHours(-10),
            new List<ProductId>
            {
                new(3),
                new(8),
                new(11),
                new(12),
                new(14),
                new(22),
                new(23),
                new(24),
            },
            false),
        TagFactory.CreateTag(
            2,
            "BOGOF",
            "Buy one get one free!",
            Now.AddDays(-2),
            Now.AddHours(-30),
            new List<ProductId>
            {
                new(2),
                new(4),
                new(10),
            },
            false),
        TagFactory.CreateTag(
            3,
            "Vegetartian",
            "No meat products",
            Now.AddDays(-2),
            Now.AddHours(-30),
            new List<ProductId>
            {
                new(9),
                new(13),
                new(15),
                new(19),
                new(20),
                new(21),
            },
            false),
    };
}
