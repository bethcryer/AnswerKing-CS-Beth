using Answer.King.Api.Extensions.DependencyInjection;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Answer.King.Api.OpenApi;

public class RemoveSchemasFilter : ISchemaFilter
{
    private readonly string[] _schemasToRemove = new[]
    {
        typeof(ProductId).CustomSchemaIdSelector(), typeof(CategoryId).CustomSchemaIdSelector()
    };

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (var schemasKey in context.SchemaRepository.Schemas.Keys)
        {
            if (this._schemasToRemove.Contains(schemasKey))
            {
                context.SchemaRepository.Schemas.Remove(schemasKey);
            }
        }
    }
}
