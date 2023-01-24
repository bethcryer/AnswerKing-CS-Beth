using Answer.King.Domain.Inventory;
using Answer.King.Domain.Repositories.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Answer.King.Api.OpenApi;

public class ProductCategorySchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(Product))
        {
            schema.Properties["category"].Items = schema.Properties["id"];
            schema.Properties["category"].Nullable = false;
        }

        if (context.Type == typeof(Category))
        {
            schema.Properties["products"].Items = schema.Properties["id"];
            schema.Properties["products"].Nullable = false;
        }
    }
}
