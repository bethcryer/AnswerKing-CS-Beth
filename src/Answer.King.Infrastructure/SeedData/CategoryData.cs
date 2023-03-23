using System;
using System.Collections.Generic;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Infrastructure.Repositories.Mappings;

namespace Answer.King.Infrastructure.SeedData;

public static class CategoryData
{
    private static readonly CategoryFactory CategoryFactory = new();

    private static readonly DateTime Now = DateTime.UtcNow;

    public static IList<Category> Categories { get; } = new List<Category>
    {
        CategoryFactory.CreateCategory(
            1,
            "Seafood",
            "Food from the oceans",
            Now.AddDays(-1),
            Now.AddHours(-10),
            new List<ProductId>
            {
                new(1),
                new(18),
            },
            false),
        CategoryFactory.CreateCategory(
            2,
            "Sundries",
            "Sides",
            Now.AddDays(-2),
            Now.AddHours(-30),
            new List<ProductId>
            {
                new(2),
                new(3),
                new(13),
                new(14),
                new(15),
            },
            false),
        CategoryFactory.CreateCategory(
            3,
            "Burgers",
            "Patty served in a sliced bun.",
            Now.AddDays(-3),
            Now.AddHours(-45),
            new List<ProductId>
            {
                new(4),
                new(5),
                new(6),
                new(7),
                new(8),
            },
            false),
        CategoryFactory.CreateCategory(
            4,
            "Pizza",
            "Stone baked flat bread with toppings.",
            Now.AddDays(-3),
            Now.AddHours(-35),
            new List<ProductId>
            {
                new(9),
                new(10),
                new(11),
                new(12),
            },
            false),
        CategoryFactory.CreateCategory(
            5,
            "Grills",
            "Flame grilled options.",
            Now.AddDays(-3),
            Now.AddHours(-25),
            new List<ProductId>
            {
                new(16),
                new(17),
            },
            false),
        CategoryFactory.CreateCategory(
            6,
            "Desserts",
            "Sweet treats.",
            Now.AddDays(-3),
            Now.AddHours(-50),
            new List<ProductId>
            {
                new(19),
                new(20),
                new(21),
            },
            false),
        CategoryFactory.CreateCategory(
            7,
            "Drinks",
            "Alcoholic and non-alcoholic beverages",
            Now.AddDays(-4),
            Now.AddHours(-15),
            new List<ProductId>
            {
                new(22),
                new(23),
                new(24),
            },
            false),
    };
}
