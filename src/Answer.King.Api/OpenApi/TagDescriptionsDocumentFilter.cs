using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Answer.King.Api.OpenApi;

public class TagDescriptionsDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags = new List<OpenApiTag> {
            new OpenApiTag { Name = "Inventory", Description = "Manage the inventory." },
            new OpenApiTag { Name = "Orders", Description = "Create and manage customer orders." }
        };
    }
}
