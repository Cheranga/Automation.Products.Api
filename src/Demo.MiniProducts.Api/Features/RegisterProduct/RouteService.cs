using System.Net.Mime;
using Demo.MiniProducts.Api.Filters;
using FluentValidation;
using Funky.Azure.DataTable.Extensions.Commands;
using Microsoft.AspNetCore.Mvc;
using Storage.Queue.Helper;

namespace Demo.MiniProducts.Api.Features.RegisterProduct;

public static class RouteService
{
    public static void Setup(RouteGroupBuilder builder)=>
    builder.MapPost(
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
}