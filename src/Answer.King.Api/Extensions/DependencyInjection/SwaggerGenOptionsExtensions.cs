using Swashbuckle.AspNetCore.SwaggerGen;

namespace Answer.King.Api.Extensions.DependencyInjection;

public static class SwaggerGenOptionsExtensions
{
    public static void UseCustomSchemaIdSelectorStrategy(this SwaggerGenOptions options)
    {
        options.CustomSchemaIds(CustomSchemaIdSelector);
    }

    internal static string CustomSchemaIdSelector(this Type modelType)
    {
        if (!modelType.IsConstructedGenericType)
        {
            var schemaId = modelType.Name.Replace("[]", "Array");
            if (modelType.Namespace!.EndsWith("RequestModels", StringComparison.OrdinalIgnoreCase))
            {
                schemaId = $"{schemaId}.Request";
            }

            if (modelType.Namespace!.Contains("Domain.Orders", StringComparison.OrdinalIgnoreCase))
            {
                schemaId = $"Orders.{schemaId}.Response";
            }

            if (modelType.Namespace!.Contains("Domain.Inventory", StringComparison.OrdinalIgnoreCase))
            {
                schemaId = $"Inventory.{schemaId}.Response";
            }

            if (modelType.Namespace!.Contains("Domain.Repositories", StringComparison.OrdinalIgnoreCase))
            {
                schemaId = $"Inventory.{schemaId}.Response";
            }

            return schemaId;
        }

        var prefix = modelType.GetGenericArguments()
            .Select(CustomSchemaIdSelector)
            .Aggregate((previous, current) => previous + current);

        var genericSchemaId = modelType.Name.Split('`').First();
        if (modelType.Namespace!.EndsWith("RequestModels", StringComparison.OrdinalIgnoreCase))
        {
            genericSchemaId = $"{genericSchemaId}.Request";
        }

        if (modelType.Namespace!.Contains("Domain.Orders", StringComparison.OrdinalIgnoreCase))
        {
            genericSchemaId = $"Orders.{genericSchemaId}.Response";
        }

        if (modelType.Namespace!.Contains("Domain.Inventory", StringComparison.OrdinalIgnoreCase))
        {
            genericSchemaId = $"Inventory.{genericSchemaId}.Response";
        }

        if (modelType.Namespace!.Contains("Domain.Repositories", StringComparison.OrdinalIgnoreCase))
        {
            genericSchemaId = $"Inventory.{genericSchemaId}.Response";
        }

        return $"{prefix}.{genericSchemaId}";
    }
}
