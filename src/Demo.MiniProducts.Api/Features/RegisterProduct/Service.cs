using System.Text.Json;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storage.Queue.Helper;
using Storage.Table.Helper;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api.Features.RegisterProduct;

public static class Service
{
    private const string Route = "products";

    public static async Task<Results<ValidationProblem, Created>> RegisterProduct(
        [FromBody] RegisterProductRequest request,
        [FromServices] IValidator<RegisterProductRequest> validator,
        [FromServices] RegisterProductSettings settings,
        [FromServices] IQueueService queueService,
        [FromServices] ITableService tableService
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return validationResult.ToValidationErrorResponse();

        var @event = new ProductRegisteredEvent(
            request.ProductId,
            request.Category,
            DateTime.UtcNow
        );

        var token = new CancellationToken();
        request.ToDataModel();

        var insertOperation = await tableService.UpsertAsync(
            settings.Category,
            settings.Table,
            request.ToDataModel(),
            token
        );

        var publishEventOperation = await queueService.PublishAsync(
            settings.Category,
            token,
            (settings.Queue, () => JsonSerializer.Serialize(@event))
        );

        return Created($"/{Route}/{request.Category}/{request.ProductId}");
    }

    private static ProductDataModel ToDataModel(this RegisterProductRequest request) =>
        ProductDataModel.New(
            request.Category,
            request.ProductId,
            request.Name,
            request.LocationCode
        );
}
