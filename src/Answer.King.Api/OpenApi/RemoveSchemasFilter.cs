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
        _ = (from schemasKey in context.SchemaRepository.Schemas.Keys
             where this._schemasToRemove.Contains(schemasKey)
             select context.SchemaRepository.Schemas.Remove(schemasKey));
    }
}
