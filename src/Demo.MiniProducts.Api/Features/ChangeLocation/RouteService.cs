using System.Net.Mime;
using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using Demo.MiniProducts.Api.Filters;
using FluentValidation;
using Funky.Azure.DataTable.Extensions.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Storage.Queue.Helper;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

public static class RouteService
{
    public static void Setup(RouteGroupBuilder builder)=>
    builder.MapPut(
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
            Service.ChangeLocation(
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
    .WithName(nameof(Service.ChangeLocation))
    .WithSummary("Update product by searching for product id.")
    .WithOpenApi(operation =>
    {
        operation.OperationId = nameof(
            Service.ChangeLocation
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
}