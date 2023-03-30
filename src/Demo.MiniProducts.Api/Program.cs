using System.Net.Mime;
using System.Runtime.InteropServices.ComTypes;
using Demo.MiniProducts.Api;
using Demo.MiniProducts.Api.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.OpenApi.Models;

var app = Bootstrapper.Setup(args);
app.UseSwagger();
app.UseSwaggerUI();
var productsApi = app.MapGroup($"/{ProductApi.Route}/")
    .WithDescription("The endpoints supported by the Products API.")
    .WithOpenApi();

productsApi
    .MapGet("", ProductApi.GetAllProducts)
    .WithSummary("Get all products.")
    .WithOpenApi(operation =>
    {
        operation.OperationId = "Get All Products";
        return operation;
    });
productsApi
    .MapGet("/{id}", ProductApi.GetProduct)
    .WithSummary("Get product by product id.")
    .WithOpenApi(operation =>
    {
        operation.OperationId = "Get Product By Id";
        var id = operation.Parameters.First();
        id.In = ParameterLocation.Path;
        id.Required = true;
        id.Description = "The id of the product.";
        return operation;
    });
;
productsApi
    .MapPost("/", ProductApi.Create)
    .Accepts<Product>(MediaTypeNames.Application.Json)
    .WithName(nameof(ProductApi.Create))
    .WithSummary("Add product.")
    .WithOpenApi();
productsApi
    .MapPut("/{id}", ProductApi.Update)
    .WithName(nameof(ProductApi.Update))
    .WithSummary("Update product by searching for product id.")
    .WithOpenApi();
;

app.Run();
