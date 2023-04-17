using System.Net.Mime;
using Demo.MiniProducts.Api.Features.ChangeLocation;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentValidation;
using Funky.Azure.DataTable.Extensions.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Storage.Queue.Helper;
using Bootstrapper = Demo.MiniProducts.Api.Bootstrapper;
using FindProduct = Demo.MiniProducts.Api.Features.FindById;
using Service = Demo.MiniProducts.Api.Features.RegisterProduct.Service;

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
    .MapPost(
        "/",
        (
            [FromBody] RegisterProductRequest request,
            [FromServices] IValidator<RegisterProductRequest> validator,
            [FromServices] RegisterProductSettings settings,
            [FromServices] IQueueService queueService,
            [FromServices] ICommandService commandService,
            [FromServices] ILoggerFactory loggerFactory
        ) =>
            Service.RegisterProduct(
                request,
                validator,
                settings,
                queueService,
                commandService,
                loggerFactory.CreateLogger("RegisterProduct")
            )
    )
    .Accepts<RegisterProductRequest>(MediaTypeNames.Application.Json)
    .WithName(nameof(Service.RegisterProduct))
    .WithSummary("Registers a product.")
    .WithOpenApi();

productsApi
    .MapPut(
        "/{category}/{id}",
        (
            ChangeLocationRequest request,
            [FromServices] IValidator<ChangeLocationRequest> validator,
            [FromServices] UpdateProductSettings updateSettings,
            [FromServices] RegisterProductSettings registerSettings,
            [FromServices] IQueueService queueService,
            [FromServices] IQueryService queryService,
            [FromServices] ICommandService commandService
        ) =>
            Demo.MiniProducts.Api.Features.ChangeLocation.Service.ChangeLocation(
                request,
                validator,
                updateSettings,
                registerSettings,
                queueService,
                queryService,
                commandService
            )
    )
    .WithName(nameof(Demo.MiniProducts.Api.Features.ChangeLocation.Service.ChangeLocation))
    .WithSummary("Update product by searching for product id.")
    .WithOpenApi();

app.Run();
