using System;
using System.Collections.Generic;
using System.Linq;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Infrastructure.SeedData;

namespace Answer.King.Infrastructure.Extensions.DependencyInjection;

public class LiteDbOptions
{
    internal List<Type> EntityMappers { get; } = new();

    internal List<Type> DataSeeders { get; } = new();

    public void RegisterEntityMappingsFromAssemblyContaining<T>()
    {
        var assembly = typeof(T).Assembly;
        var types = assembly.GetTypes()
            .Where(t =>
                typeof(IEntityMapping).IsAssignableFrom(t)
                && !t.IsAbstract
                && !t.IsInterface).ToList();

        this.EntityMappers.AddRange(types);
    }

    public void RegisterDataSeedingFromAssemblyContaining<T>()
    {
        var assembly = typeof(T).Assembly;
        var types = assembly.GetTypes()
            .Where(t =>
                typeof(ISeedData).IsAssignableFrom(t)
                && !t.IsAbstract
                && !t.IsInterface).ToList();

        this.DataSeeders.AddRange(types);
    }
}
