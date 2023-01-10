using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Infrastructure.Repositories.Mappings;

internal class TagFactory
{
    private ConstructorInfo? TagConstructor { get; } = typeof(Tag)
        .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
        .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

    public Tag CreateTag(
        long id,
        string name,
        string description,
        DateTime createdOn,
        DateTime lastUpdated,
        IList<ProductId> products,
        bool retired)
    {
        var parameters = new object[]
        {
            id,
            name,
            description,
            createdOn,
            lastUpdated,
            products,
            retired
        };

        /* invoking a private constructor will wrap up any exception into a
         * TargetInvocationException so here I unwrap it
         */
        try
        {
            return (Tag)this.TagConstructor?.Invoke(parameters)!;
        }
        catch (Exception ex)
        {
            var exception = ex.InnerException ?? ex;
            throw exception;
        }
    }
}
