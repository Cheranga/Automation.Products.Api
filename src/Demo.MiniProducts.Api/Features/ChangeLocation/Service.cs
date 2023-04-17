using System.Text.Json;
using Azure.Storage.Table.Wrapper.Commands;
using Azure.Storage.Table.Wrapper.Queries;
using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storage.Queue.Helper;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using static Microsoft.AspNetCore.Http.TypedResults;
using QR = Azure.Storage.Table.Wrapper.Queries.QueryResponse<
    Azure.Storage.Table.Wrapper.Queries.QueryResult.QueryFailedResult,
    Azure.Storage.Table.Wrapper.Queries.QueryResult.EmptyResult,
    Azure.Storage.Table.Wrapper.Queries.QueryResult.SingleResult<Demo.MiniProducts.Api.DataAccess.ProductDataModel>
>;

using CR = Azure.Storage.Table.Wrapper.Commands.CommandResponse<
    Azure.Storage.Table.Wrapper.Commands.CommandOperation.CommandFailedOperation,
    Azure.Storage.Table.Wrapper.Commands.CommandOperation.CommandSuccessOperation
>;

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
    ) =>
        (
            await (
                from vOp in ValidateRequest(request, validator, token)
                from getOp in GetProductFromTable(
                    registerSettings.Category,
                    registerSettings.Table,
                    request.Category.ToUpper(),
                    request.Id.ToUpper(),
                    queryService,
                    token
                )
                from dataModel in ToProductDataModel(getOp).ToEff()
                from updateOp in UpdateProductLocation(
                    registerSettings.Category,
                    registerSettings.Table,
                    ProductDataModel.New(
                        dataModel.Category,
                        dataModel.ProductId,
                        dataModel.Name,
                        request.LocationCode
                    ),
                    commandService,
                    token
                )
                from publishOp in PublishEvent(
                    updateSettings.Category,
                    updateSettings.Queue,
                    () =>
                        JsonSerializer.Serialize(
                            new LocationChangedEvent(
                                request.Id,
                                dataModel.LocationCode,
                                request.LocationCode
                            )
                        ),
                    queueService,
                    token
                )
                select publishOp
            ).Run()
        ).Match(_ => NoContent(), GetErrorResponse);

    private static Results<
        ValidationProblem,
        ProblemHttpResult,
        NotFound,
        NoContent
    > GetErrorResponse(Error error) =>
        error switch
        {
            ApiError<ValidationResult> ve => ve.Data.ToValidationErrorResponse(),
            ApiError<QueryResult.EmptyResult> => NotFound(),
            ApiError<CommandOperation.CommandFailedOperation> ce
                => Error.New(ce.Data.ErrorCode, ce.Data.ErrorMessage).ToErrorResponse(),
            ApiError<QueueOperation.FailedOperation> qe
                => Error.New(qe.Data.Error.Code, qe.Data.Error.Message).ToErrorResponse(),
            _
                => Problem(
                    new ProblemDetails
                    {
                        Type = "Error",
                        Title = "Error",
                        Detail = error.Message,
                        Status = StatusCodes.Status500InternalServerError
                    }
                )
        };

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

    private static Aff<ValidationResult> ValidateRequest(
        ChangeLocationRequest request,
        IValidator<ChangeLocationRequest> validator,
        CancellationToken token
    ) =>
        from vr in AffMaybe<ValidationResult>(
            async () => await validator.ValidateAsync(request, token)
        )
        from _ in guard(vr.IsValid, ApiError<ValidationResult>.New(vr))
        select vr;

    private static Aff<QR> GetProductFromTable(
        string category,
        string table,
        string partitionKey,
        string rowKey,
        IQueryService queryService,
        CancellationToken token
    ) =>
        from op in Aff(
            async () =>
                await queryService.GetEntityAsync<ProductDataModel>(
                    category,
                    table,
                    partitionKey,
                    rowKey,
                    token
                )
        )
        from _ in guardnot(
            op.Response is QueryResult.EmptyResult,
            ApiError<QueryResult.EmptyResult>.New((op.Response as QueryResult.EmptyResult)!)
        )
        select op;

    private static Aff<CR> UpdateProductLocation(
        string category,
        string table,
        ProductDataModel dataModel,
        ICommandService commandService,
        CancellationToken token
    ) =>
        from op in Aff(
            async () => await commandService.UpdateAsync(category, table, dataModel, token)
        )
        from _ in guardnot(
            op.Operation is CommandOperation.CommandFailedOperation,
            ApiError<CommandOperation.CommandFailedOperation>.New(
                (op.Operation as CommandOperation.CommandFailedOperation)!
            )
        )
        select op;

    private static Fin<ProductDataModel> ToProductDataModel(this QR queryResponse) =>
        queryResponse.Response switch
        {
            QueryResult.EmptyResult er
                => Fin<ProductDataModel>.Fail(ApiError<QueryResult.EmptyResult>.New(er)),
            QueryResult.SingleResult<ProductDataModel> sr => Fin<ProductDataModel>.Succ(sr.Entity),
            QueryResult.CollectionResult<ProductDataModel> cr
                => Fin<ProductDataModel>.Succ(cr.Entities.First()),
            _ => Fin<ProductDataModel>.Fail(Error.New("unsupported"))
        };

    private static Aff<QueueOperation> PublishEvent(
        string category,
        string queue,
        Func<string> contentFunc,
        IQueueService queueService,
        CancellationToken token
    ) =>
        from op in Aff(
            async () => await queueService.PublishAsync(category, token, (queue, contentFunc))
        )
        from _ in guardnot(
            op is QueueOperation.FailedOperation,
            ApiError<QueueOperation.FailedOperation>.New((op as QueueOperation.FailedOperation)!)
        )
        select op;
}
