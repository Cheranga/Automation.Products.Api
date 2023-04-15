using System.Text.Json;
using Azure.Storage.Table.Wrapper.Commands;
using Azure.Storage.Table.Wrapper.Queries;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storage.Queue.Helper;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

public static class Service
{
    public static async Task<
        Results<ValidationProblem, ProblemHttpResult, NotFound, NoContent>
    > ChangeLocation(
        ChangeLocationRequest request,
        [FromServices] IValidator<ChangeLocationRequest> validator,
        [FromServices] UpdateProductSettings updateSettings,
        [FromServices] RegisterProductSettings registerSettings,
        [FromServices] IQueueService queueService,
        [FromServices] IQueryService queryService,
        [FromServices] ICommandService commandService
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
            queryService,
            commandService,
            new CancellationToken()
        );
    }

    private static async Task<
        Results<ValidationProblem, ProblemHttpResult, NotFound, NoContent>
    > UpdateLocation(
        ChangeLocationRequest request,
        UpdateProductSettings updatedSettings,
        RegisterProductSettings registerSettings,
        IQueueService queueService,
        IQueryService queryService,
        ICommandService tableService,
        CancellationToken token
    )
    {
        var getProductOperation = await queryService.GetEntityAsync<ProductDataModel>(
            registerSettings.Category,
            registerSettings.Table,
            request.Category.ToUpper(),
            request.Id.ToUpper(),
            token
        );

        if (getProductOperation.Response is QueryResult.QueryFailedResult f)
        {
            return Problem(
                new ProblemDetails
                {
                    Type = "Error",
                    Title = f.ErrorCode.ToString(),
                    Detail = f.ErrorMessage,
                    Status = StatusCodes.Status500InternalServerError
                }
            );
        }

        if (getProductOperation.Response is QueryResult.EmptyResult)
        {
            return NotFound();
        }

        var product = (
            getProductOperation.Response as QueryResult.SingleResult<ProductDataModel>
        )!.Entity;

        var updatedProduct = ProductDataModel.New(
            product.Category,
            product.ProductId,
            product.Name,
            request.LocationCode
        );
        await UpdateLocation(updatedProduct, registerSettings, tableService, token);

        await PublishLocationChangedEvent(
            product.LocationCode,
            request,
            updatedSettings,
            queueService,
            token
        );

        return NoContent();
    }

    private static async Task UpdateLocation(
        ProductDataModel product,
        RegisterProductSettings registerSettings,
        ICommandService commandService,
        CancellationToken token
    )
    {
        await commandService.UpdateAsync(
            registerSettings.Category,
            registerSettings.Table,
            product,
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
