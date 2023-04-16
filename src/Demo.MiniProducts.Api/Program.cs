using System.ComponentModel;
using System.Net.Mime;
using Azure.Storage.Table.Wrapper.Queries;
using Demo.MiniProducts.Api;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using RegisterProduct = Demo.MiniProducts.Api.Features.RegisterProduct;
using ChangeLocation = Demo.MiniProducts.Api.Features.ChangeLocation;
using FindProduct = Demo.MiniProducts.Api.Features.FindById;

const string Route = "products";

var app = Bootstrapper.Setup(args);
app.UseSwagger();
app.UseSwaggerUI();
var productsApi = app.MapGroup($"/{Route}/").WithOpenApi();

productsApi
    .MapGet(
        "/{category}/{id}",
        (
            [FromRoute] string category,
            [FromRoute] string id,
            [FromServices] RegisterProductSettings settings,
            [FromServices] IQueryService queryService,
            [FromServices] ILoggerFactory loggerFactory
        ) =>
            FindProduct.Service.GetProductDetailsById(
                category,
                id,
                settings,
                queryService,
                loggerFactory.CreateLogger("FindProductsById")
            )
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
    .MapPost("/", RegisterProduct.Service.RegisterProduct)
    .Accepts<RegisterProductRequest>(MediaTypeNames.Application.Json)
    .WithName(nameof(RegisterProduct.Service.RegisterProduct))
    .WithSummary("Registers a product.")
    .WithOpenApi();

productsApi
    .MapPut("/{category}/{id}", ChangeLocation.Service.ChangeLocation)
    .WithName(nameof(ChangeLocation.Service.ChangeLocation))
    .WithSummary("Update product by searching for product id.")
    .WithOpenApi();

app.Run();
