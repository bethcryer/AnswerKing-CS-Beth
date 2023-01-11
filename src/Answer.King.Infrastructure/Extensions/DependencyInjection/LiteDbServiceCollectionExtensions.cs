using System;
using System.Linq;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Infrastructure.SeedData;
using LiteDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Answer.King.Infrastructure.Extensions.DependencyInjection;

public static class LiteDbServiceCollectionExtensions
{
    public static IServiceCollection ConfigureLiteDb(this IServiceCollection services, Action<LiteDbOptions>? setupAction = null)
    {
        var action = setupAction ?? delegate { };
        var options = new LiteDbOptions();

        action(options);

        options.EntityMappers.ForEach(type => services.Add(ServiceDescriptor.Transient(typeof(IEntityMapping), type)));
        options.DataSeeders.ForEach(type => services.Add(ServiceDescriptor.Transient(typeof(ISeedData), type)));

        services.AddSingleton<BsonMapper>();
        services.AddSingleton<ILiteDbConnectionFactory, LiteDbConnectionFactory>();

        return services;
    }

    public static IApplicationBuilder UseLiteDb(this IApplicationBuilder app)
    {
        var mapperInstance = app.ApplicationServices.GetService<BsonMapper>()!;
        mapperInstance.UseCamelCase();

        var mappings =
            app.ApplicationServices.GetServices<IEntityMapping>().ToList();

        foreach (var entityMapping in mappings)
        {
            entityMapping.RegisterMapping(mapperInstance);
        }

        var originalResolver = mapperInstance.ResolveMember;

        mapperInstance.ResolveMember = (type, memberInfo, memberMapper) =>
        {
            originalResolver(type, memberInfo, memberMapper);
            mappings.ForEach(mapping => mapping.ResolveMember(type, memberInfo, memberMapper));
        };

        var connections = app.ApplicationServices.GetRequiredService<ILiteDbConnectionFactory>();

        var seeders = app.ApplicationServices.GetServices<ISeedData>();
        foreach (var seedData in seeders)
        {
            seedData.SeedData(connections);
        }

        return app;
    }
}
