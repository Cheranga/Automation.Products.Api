using System.Reflection;
using System.Text.Json;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storage.Queue.Helper;
using Storage.Table.Helper;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

public class ChangeLocationRequest
{
    public string Category { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;

    public static async ValueTask<ChangeLocationRequest> BindAsync(
        HttpContext context,
        ParameterInfo _
    )
    {
        var record = await context.Request.Body.ToModel<ChangeLocationRequest>();
        return record;
    }
}

public static class Service
{
    public static async Task<Results<ValidationProblem, NotFound, NoContent>> ChangeLocation(
        ChangeLocationRequest request,
        [FromServices] IValidator<ChangeLocationRequest> validator,
        [FromServices] UpdateProductSettings updateSettings,
        [FromServices] RegisterProductSettings registerSettings,
        [FromServices] IQueueService queueService,
        [FromServices] ITableService tableService
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return validationResult.ToValidationErrorResponse();

        return await UpdateLocation(
            request,
            updateSettings,
            registerSettings,
            queueService,
            tableService,
            new CancellationToken()
        );
    }

    private static async Task<Results<ValidationProblem, NotFound, NoContent>> UpdateLocation(
        ChangeLocationRequest request,
        UpdateProductSettings updatedSettings,
        RegisterProductSettings registerSettings,
        IQueueService queueService,
        ITableService tableService,
        CancellationToken token
    )
    {
        var getProductOperation = await tableService.GetAsync<ProductDataModel>(
            registerSettings.Category,
            registerSettings.Table,
            request.Category,
            request.Id,
            token
        );

        if (getProductOperation is TableOperation.FailedOperation)
            return NotFound();

        var product = (
            getProductOperation as TableOperation.SuccessOperation<ProductDataModel>
        )!.Data;

        await UpdateLocation(
            product,
            request,
            registerSettings,
            tableService,
            token
        );

        await PublishLocationChangedEvent(
            product.LocationCode,
            request,
            updatedSettings,
            queueService,
            token
        );

        return NoContent();
    }

    private static async Task UpdateLocation(ProductDataModel product,
        ChangeLocationRequest request,
        RegisterProductSettings registerSettings,
        ITableService tableService,
        CancellationToken token)
    {
        await tableService.UpsertAsync(
            registerSettings.Category,
            registerSettings.Table,
            request.ToDataModel(product.Name, request.LocationCode),
            true,
            token
        );
    }

    private static async Task PublishLocationChangedEvent(
        string previousLocationCode,
        ChangeLocationRequest request,
        UpdateProductSettings updatedSettings,
        IQueueService queueService,
        CancellationToken token
    )
    {
        var @event = new LocationChangedEvent(
            request.Id,
            previousLocationCode,
            request.LocationCode
        );

        await queueService.PublishAsync(
            updatedSettings.Category,
            token,
            (updatedSettings.Queue, () => JsonSerializer.Serialize(@event))
        );
    }

    private static ProductDataModel ToDataModel(
        this ChangeLocationRequest request,
        string name,
        string locationCode
    ) => ProductDataModel.New(request.Category, request.Id, name, locationCode);
}
