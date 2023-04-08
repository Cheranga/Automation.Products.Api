using System.Net.Mime;
using Demo.MiniProducts.Api;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using Microsoft.OpenApi.Models;

const string Route = "products";

var app = Bootstrapper.Setup(args);
app.UseSwagger();
app.UseSwaggerUI();
var productsApi = app.MapGroup($"/{Route}/").WithOpenApi();

productsApi
    .MapGet(
        "/{category}/{id}",
        Demo.MiniProducts.Api.Features.FindById.Service.GetProductDetailsById
    )
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
    .MapPost("/", Service.RegisterProduct)
    .Accepts<RegisterProductRequest>(MediaTypeNames.Application.Json)
    .WithName(nameof(Service.RegisterProduct))
    .WithSummary("Registers a product.")
    .WithOpenApi();

productsApi
    .MapPut("/", Demo.MiniProducts.Api.Features.ChangeLocation.Service.ChangeLocation)
    .WithName(nameof(Demo.MiniProducts.Api.Features.ChangeLocation.Service.ChangeLocation))
    .WithSummary("Update product by searching for product id.")
    .WithOpenApi();

app.Run();
