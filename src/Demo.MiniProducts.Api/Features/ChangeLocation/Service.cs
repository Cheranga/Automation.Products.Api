using System.Text.Json;
using Azure.Storage.Table.Wrapper.Commands;
using Azure.Storage.Table.Wrapper.Queries;
using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storage.Queue.Helper;
using static Microsoft.AspNetCore.Http.TypedResults;
using static Demo.MiniProducts.Api.Extensions.ApiOperationExtensions;

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
        [FromServices] ICommandService commandService,
        CancellationToken token = new()
    )
    {
        var validateOperation = await Validate(request, validator, token);
        if (validateOperation.Operation is ApiOperation.ApiValidationFailureOperation vf)
            return vf.ValidationResult.ToValidationErrorResponse();

        var getProductOperation = await GetEntity<ProductDataModel>(
            registerSettings.Category,
            registerSettings.Table,
            request.Category.ToUpper(),
            request.Id.ToUpper(),
            queryService,
            token
        );
        if (getProductOperation.Operation is ApiOperation.ApiFailedOperation gf)
            return gf.ToErrorResponse();

        if (getProductOperation.Operation is ApiOperation.ApiSuccessfulOperation)
            return NotFound();

        var product = (
            getProductOperation.Operation as ApiOperation.ApiSuccessfulOperation<ProductDataModel>
        )!.Data;

        var updatedProduct = ProductDataModel.New(
            product.Category,
            product.ProductId,
            product.Name,
            request.LocationCode
        );
        var updateOperation = await UpdateLocation(
            updatedProduct,
            registerSettings,
            commandService,
            token
        );
        if (updateOperation.Operation is ApiOperation.ApiFailedOperation f)
            return f.ToErrorResponse();

        var publishOperation = await PublishLocationChangedEvent(
            product.LocationCode,
            request,
            updateSettings,
            queueService,
            token
        );

        if (publishOperation.Operation is ApiOperation.ApiFailedOperation pf)
            return pf.ToErrorResponse();

        return NoContent();
    }

    private static async Task<
        ApiOperationResult<ApiOperation.ApiFailedOperation, ApiOperation.ApiSuccessfulOperation>
    > UpdateLocation(
        ProductDataModel product,
        RegisterProductSettings registerSettings,
        ICommandService commandService,
        CancellationToken token
    )
    {
        var op = await commandService.UpdateAsync(
            registerSettings.Category,
            registerSettings.Table,
            product,
            token
        );

        return op.Operation switch
        {
            CommandOperation.CommandFailedOperation f
                => ApiOperation.Fail(f.ErrorCode, f.ErrorMessage, f.Exception),
            CommandOperation.CommandSuccessOperation => ApiOperation.Success(),
            _ => throw new NotSupportedException()
        };
    }

    private static async Task<
        ApiOperationResult<ApiOperation.ApiFailedOperation, ApiOperation.ApiSuccessfulOperation>
    > PublishLocationChangedEvent(
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

        var op = await queueService.PublishAsync(
            updatedSettings.Category,
            token,
            (updatedSettings.Queue, () => JsonSerializer.Serialize(@event))
        );

        return op switch
        {
            QueueOperation.FailedOperation f
                => ApiOperation.Fail(f.Error.Code, f.Error.Message, f.Error.ToException()),
            QueueOperation.SuccessOperation => ApiOperation.Success(),
            _ => throw new NotSupportedException()
        };
    }
}
