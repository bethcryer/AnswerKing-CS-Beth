using Answer.King.Api.Extensions.DependencyInjection;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Answer.King.Api.OpenApi;

public class RemoveSchemasFilter : ISchemaFilter
{
    private readonly string[] schemasToRemove = new[]
    {
        typeof(ProductId).CustomSchemaIdSelector(), typeof(CategoryId).CustomSchemaIdSelector(),
    };

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        _ = context.SchemaRepository.Schemas.Keys.Where(schemasKey => this.schemasToRemove.Contains(schemasKey))
            .Select(schemasKey => context.SchemaRepository.Schemas.Remove(schemasKey));
    }
}
