using System.Net.Mime;
using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Features.ChangeLocation;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using Demo.MiniProducts.Api.Filters;
using FluentValidation;
using Funky.Azure.DataTable.Extensions.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Serilog;
using Storage.Queue.Helper;
using Bootstrapper = Demo.MiniProducts.Api.Bootstrapper;
using FindProduct = Demo.MiniProducts.Api.Features.FindById;
using Service = Demo.MiniProducts.Api.Features.RegisterProduct.Service;
using Validator = Demo.MiniProducts.Api.Features.RegisterProduct.Validator;

const string Route = "products";

var app = Bootstrapper.Setup(args);
app.UseSerilogRequestLogging();
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
                new FindProduct.GetProductByIdRequest(category, id),
                settings,
                queryService,
                loggerFactory.CreateLogger("FindProductsById")
            )
    )
    .WithName(nameof(FindProduct.Service.GetProductDetailsById))
    .WithSummary("Get product by product id.")
    .WithOpenApi(operation =>
    {
        operation.OperationId = nameof(FindProduct.Service.GetProductDetailsById);
        return operation
            .SetParamInfo<FindProduct.GetProductByIdRequest>(
                x => x.Category,
                x =>
                {
                    x.In = ParameterLocation.Path;
                    x.Required = true;
                    x.Description = "The category of the product.";
                    x.Example = new OpenApiString("TECH");
                }
            )
            .SetParamInfo<FindProduct.GetProductByIdRequest>(
                x => x.Id,
                x =>
                {
                    x.In = ParameterLocation.Path;
                    x.Required = true;
                    x.Description = "The id of the product.";
                    x.Example = new OpenApiString("PROD1");
                }
            );
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
    .AddEndpointFilter<PerformanceFilter<RegisterProductRequest>>()
    .AddEndpointFilter<RequestValidationFilter<RegisterProductRequest, Validator>>()
    .Accepts<RegisterProductRequest>(false, MediaTypeNames.Application.Json)
    .WithName(nameof(Service.RegisterProduct))
    .WithSummary("Registers a product.")
    .WithOpenApi();

productsApi
    .MapPut(
        "location/{category}/{id}",
        (
            [FromRoute] string category,
            [FromRoute] string id,
            [FromBody] ChangeLocationRequestDto request,
            [FromServices] IValidator<ChangeLocationRequest> validator,
            [FromServices] UpdateProductSettings updateSettings,
            [FromServices] RegisterProductSettings registerSettings,
            [FromServices] IQueueService queueService,
            [FromServices] IQueryService queryService,
            [FromServices] ICommandService commandService
        ) =>
            Demo.MiniProducts.Api.Features.ChangeLocation.Service.ChangeLocation(
                new ChangeLocationRequest(category, id, request.LocationCode),
                validator,
                updateSettings,
                registerSettings,
                queueService,
                queryService,
                commandService
            )
    )
    .AddEndpointFilter<PerformanceFilter<ChangeLocationRequestDto>>()
    .AddEndpointFilter<RequestValidationFilter<ChangeLocationRequestDto, ChangeLocationRequestDtoValidator>>()
    .Accepts<ChangeLocationRequest>(false, MediaTypeNames.Application.Json)
    .WithName(nameof(Demo.MiniProducts.Api.Features.ChangeLocation.Service.ChangeLocation))
    .WithSummary("Update product by searching for product id.")
    .WithOpenApi(operation =>
    {
        operation.OperationId = nameof(
            Demo.MiniProducts.Api.Features.ChangeLocation.Service.ChangeLocation
        );
        return operation
            .SetParamInfo<ChangeLocationRequest>(
                x => x.Category,
                x =>
                {
                    x.In = ParameterLocation.Path;
                    x.Required = true;
                    x.Description = "The category of the product.";
                    x.Example = new OpenApiString("TECH");
                }
            )
            .SetParamInfo<ChangeLocationRequest>(
                x => x.Id,
                x =>
                {
                    x.In = ParameterLocation.Path;
                    x.Required = true;
                    x.Description = "The id of the product.";
                    x.Example = new OpenApiString("PROD1");
                }
            );
    });

app.Run();
