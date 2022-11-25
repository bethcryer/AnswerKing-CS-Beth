using System.Reflection;
using System.Text.Json.Serialization;
using Answer.King.Api.Common.JsonConverters;
using Answer.King.Api.Common.Validators;
using Answer.King.Api.Extensions.DependencyInjection;
using Answer.King.Api.OpenApi;
using Answer.King.Api.Services;
using Answer.King.Domain.Repositories;
using Answer.King.Infrastructure.Extensions.DependencyInjection;
using Answer.King.Infrastructure.Repositories;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Infrastructure.SeedData;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var corsAllowAnyPolicy = "AllowAnyOrigin";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsAllowAnyPolicy,
        policy =>
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new CategoryIdJsonConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.UseCustomSchemaIdSelectorStrategy();

    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Answer King API", Version = "v1" });
    options.DocumentFilter<TagDescriptionsDocumentFilter>();
    options.SchemaFilter<ValidationProblemDetailsSchemaFilter>();
    options.SchemaFilter<EnumSchemaFilter>();
    options.SchemaFilter<ProductCategorySchemaFilter>();
    options.SchemaFilter<RemoveSchemasFilter>();

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
    options.EnableAnnotations();
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(
    _ => ValidatorOptions.Global.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName);

builder.Services.ConfigureLiteDb(options =>
{
    options.RegisterEntityMappingsFromAssemblyContaining<IEntityMapping>();
    options.RegisterDataSeedingFromAssemblyContaining<ISeedData>();
});

builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IPaymentRepository, PaymentRepository>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Answer King API V1"));
}

app.UseLiteDb();
app.UseHttpsRedirection();
app.UseCors(corsAllowAnyPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();
