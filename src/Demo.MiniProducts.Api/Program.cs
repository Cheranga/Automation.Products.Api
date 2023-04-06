using System.Net.Mime;
using System.Runtime.InteropServices.ComTypes;
using Demo.MiniProducts.Api;
using Demo.MiniProducts.Api.Models;
using Demo.MiniProducts.Api.Responses;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.OpenApi.Models;

var app = Bootstrapper.Setup(args);
app.UseSwagger();
app.UseSwaggerUI();
var productsApi = app.MapGroup($"/{ProductApi.Route}/")
    .WithOpenApi();

productsApi
    .MapGet("", ProductApi.GetAllProducts)
    .Produces<ProductListResponse>()
    .WithSummary("Get all products.")
    .WithOpenApi(operation =>
    {
        operation.OperationId = "Get All Products";
        return operation;
    });

productsApi
    .MapGet("/{id}", ProductApi.GetProductDetailsById)
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

productsApi
    .MapPost("/", ProductApi.RegisterProduct)
    .Accepts<Product>(MediaTypeNames.Application.Json)
    .WithName(nameof(ProductApi.RegisterProduct))
    .WithSummary("Registers a product.")
    .WithOpenApi();

// productsApi
//     .MapPut("/{id}", ProductApi.ChangeLocation)
//     .WithName(nameof(ProductApi.ChangeLocation))
//     .WithSummary("Update product by searching for product id.")
//     .WithOpenApi();

app.Run();
