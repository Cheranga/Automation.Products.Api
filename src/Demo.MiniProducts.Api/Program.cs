using System.Runtime.InteropServices.ComTypes;
using Demo.MiniProducts.Api;
using Microsoft.OpenApi.Models;

var app = Bootstrapper.Setup(args);
app.UseSwagger();
app.UseSwaggerUI();
var productsApi = app.MapGroup($"/{ProductApi.Route}/");

productsApi
    .MapGet("", ProductApi.GetAllProducts)
    .WithName(nameof(ProductApi.GetAllProducts))
    .WithOpenApi();
productsApi
    .MapGet("/{id}", ProductApi.GetProduct)
    .WithName(nameof(ProductApi.GetProduct))
    .WithOpenApi(operation =>
    {
        var id = operation.Parameters.First();
        id.In = ParameterLocation.Path;
        id.Required = true;
        id.Description = "The id of the product.";
        return operation;
    });
;
productsApi.MapPost("/", ProductApi.Create).WithName(nameof(ProductApi.Create)).WithOpenApi();
productsApi.MapPut("/{id}", ProductApi.Update).WithName(nameof(ProductApi.Update)).WithOpenApi();
;

app.Run();
