using System;
using System.Collections.Generic;
using System.Linq;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;

namespace Answer.King.Infrastructure.SeedData;

internal static class ProductData
{
    private static readonly ProductFactory ProductFactory = new();

    private static readonly DateTime Now = DateTime.UtcNow;

    public static IList<Product> Products { get; } = new List<Product>
    {
        ProductFactory.CreateProduct(
            1,
            "Fish",
            "Delicious and satisfying.",
            5.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(1),
            Array.Empty<TagId>(),
            false),
        ProductFactory.CreateProduct(
            2,
            "Chips",
            "Nothing more to say.",
            2.99,
            Now.AddDays(-2),
            Now.AddHours(-15),
            Category(2),
            Tags(2),
            false),
        ProductFactory.CreateProduct(
            3,
            "Gravy",
            "Side",
            0.99,
            Now.AddDays(-10),
            Now.AddHours(-5),
            Category(2),
            Tags(1),
            true),
        ProductFactory.CreateProduct(
            4,
            "Plain Burger",
            "Plain beef patty served in a sliced bun",
            8.99,
            Now.AddDays(-30),
            Now.AddHours(-8),
            Category(3),
            Tags(2),
            false),
        ProductFactory.CreateProduct(
            5,
            "Cheese Burger",
            "Plain burger with cheese",
            9.99,
            Now.AddDays(-15),
            Now.AddHours(-20),
            Category(3),
            Array.Empty<TagId>(),
            false),
        ProductFactory.CreateProduct(
            6,
            "Bacon Burger",
            "Plain burger with bacon",
            9.99,
            Now.AddDays(-20),
            Now.AddHours(-5),
            Category(3),
            Array.Empty<TagId>(),
            false),
        ProductFactory.CreateProduct(
            7,
            "Chicken Burger",
            "Breadcrumbed chicken breast served in a sliced bun",
            8.50,
            Now.AddDays(-35),
            Now.AddHours(-15),
            Category(3),
            Array.Empty<TagId>(),
            false),
        ProductFactory.CreateProduct(
            8,
            "Plant Burger",
            "Plant based patty served in a sliced bun",
            9.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(3),
            Tags(1),
            false),
        ProductFactory.CreateProduct(
            9,
            "Margherita",
            "Tomato and mozarella with fresh basil",
            10.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(4),
            Tags(3),
            false),
        ProductFactory.CreateProduct(
            10,
            "Pepperoni",
            "Tomato, mozzarella and pepperoni",
            11.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(4),
            Tags(2),
            false),
        ProductFactory.CreateProduct(
            11,
            "Vegan Club",
            "This Isn’t bacon, vegan chick’n pieces, vegan mozzarella and fresh tomatoes",
            12.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(4),
            Tags(1),
            false),
        ProductFactory.CreateProduct(
            12,
            "Spicy Shawarma",
            "Tomato and chilli sauce, vegan shawarma, vegan mozzarella, green peppers and jalapenos",
            12.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(4),
            Tags(1),
            false),
        ProductFactory.CreateProduct(
            13,
            "Nachos",
            "Corn tortilla chips, with cheese, jalapeños, guacamole, spring onions, coriander and salsa",
            2.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(2),
            Tags(3),
            false),
        ProductFactory.CreateProduct(
            14,
            "Potato Wedges",
            "Baked southern spiced potato wedges",
            2.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(2),
            Tags(1),
            false),
        ProductFactory.CreateProduct(
            15,
            "Onion Rings",
            "Battered, deep fried onion rings",
            1.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(2),
            Tags(3),
            false),
        ProductFactory.CreateProduct(
            16,
            "Sirloin Steak",
            "Locally sourced 8oz sirloin",
            16.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(5),
            Array.Empty<TagId>(),
            false),
        ProductFactory.CreateProduct(
            17,
            "Rump Steak",
            "Locally sourced 10oz rump",
            14.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(5),
            Array.Empty<TagId>(),
            false),
        ProductFactory.CreateProduct(
            18,
            "Scampi",
            "Breaded scampi",
            7.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(1),
            Array.Empty<TagId>(),
            true),
        ProductFactory.CreateProduct(
            19,
            "Cheesecake",
            "Breadcrumb and cream cheese dessert",
            4.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(6),
            Tags(3),
            false),
        ProductFactory.CreateProduct(
            20,
            "Ice Cream",
            "Frozen dessert",
            4.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(6),
            Tags(3),
            false),
        ProductFactory.CreateProduct(
            21,
            "Popcorn Sundae",
            "Ice cream, chocolate fudge and popcorn",
            6.99,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(6),
            Tags(3),
            false),
        ProductFactory.CreateProduct(
            22,
            "Lager",
            "Alcoholic",
            2.50,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(7),
            Tags(1),
            false),
        ProductFactory.CreateProduct(
            23,
            "Wine",
            "Alcoholic",
            4.50,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(7),
            Tags(1),
            false),
        ProductFactory.CreateProduct(
            24,
            "Cocktail",
            "Alcoholic",
            7.50,
            Now.AddDays(-1),
            Now.AddHours(-10),
            Category(7),
            Tags(1),
            false),
    };

    private static ProductCategory Category(long id)
    {
        return CategoryData.Categories.Where(c => c.Id == id)
            .Select(x => new ProductCategory(x.Id, x.Name, x.Description)).SingleOrDefault()!;
    }

    private static IList<TagId> Tags(long id)
    {
        return TagData.Tags.Where(c => c.Id == id)
            .Select(x => new TagId(x.Id)).ToList();
    }
}
