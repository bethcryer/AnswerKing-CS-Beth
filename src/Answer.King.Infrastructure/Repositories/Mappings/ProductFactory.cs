using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Answer.King.Domain.Repositories.Models;

[assembly: InternalsVisibleTo("Answer.King.Domain.UnitTests")]

namespace Answer.King.Infrastructure.Repositories.Mappings;

internal class ProductFactory
{
    private ConstructorInfo? ProductConstructor { get; } = typeof(Product)
        .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
        .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

    public Product CreateProduct(
        long id,
        string name,
        string description,
        double price,
        IList<CategoryId> categories,
        IList<TagId> tags,
        bool retired)
    {
        var parameters = new object[] { id, name, description, price, categories, tags, retired };

        /* invoking a private constructor will wrap up any exception into a
         * TargetInvocationException so here I unwrap it
         */
        try
        {
            return (Product)this.ProductConstructor?.Invoke(parameters)!;
        }
        catch (Exception ex)
        {
            var exception = ex.InnerException ?? ex;
            throw exception;
        }
    }
}
