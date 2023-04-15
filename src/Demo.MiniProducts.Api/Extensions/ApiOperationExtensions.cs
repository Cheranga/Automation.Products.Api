using Azure.Data.Tables;
using Azure.Storage.Table.Wrapper.Commands;
using Azure.Storage.Table.Wrapper.Queries;
using Demo.MiniProducts.Api.Core;
using FluentValidation;
using Storage.Queue.Helper;

namespace Demo.MiniProducts.Api.Extensions;

public static class ApiOperationExtensions
{
    public static async Task<
        ApiOperationResult<
            ApiOperation.ApiValidationFailureOperation,
            ApiOperation.ApiSuccessfulOperation
        >
    > Validate<T>(T data, IValidator<T> validator, CancellationToken token = new())
    {
        var validationResult = await validator.ValidateAsync(data, token);
        return validationResult.IsValid
            ? ApiOperation.Success()
            : ApiOperation.FailedValidation(validationResult);
    }

    public static async Task<
        ApiOperationResult<ApiOperation.ApiFailedOperation, ApiOperation.ApiSuccessfulOperation>
    > Upsert<T>(
        string category,
        string table,
        T data,
        ICommandService commandService,
        CancellationToken token
    ) where T : class, ITableEntity
    {
        var insertOperation = await commandService.UpsertAsync(category, table, data, token);

        return insertOperation.Operation switch
        {
            CommandOperation.CommandFailedOperation f
                => ApiOperation.Fail(f.ErrorCode, f.ErrorMessage, f.Exception),
            CommandOperation.CommandSuccessOperation => ApiOperation.Success(),
            _ => throw new NotSupportedException()
        };
    }

    public static async Task<ApiOperationResult<ApiOperation.ApiFailedOperation, ApiOperation.ApiSuccessfulOperation, ApiOperation.ApiSuccessfulOperation<T>>> GetEntity<T>(
        string category,
        string table,
        string partitionKey,
        string rowKey,
        IQueryService queryService,
        CancellationToken token = new()
    )
    where T:class, ITableEntity
    {
        var op = await queryService.GetEntityAsync<T>(
            category,
            table,
            partitionKey,
            rowKey,
            token
        );

        return op.Response switch
        {
            QueryResult.EmptyResult => ApiOperation.Success(),
            QueryResult.SingleResult<T> t => ApiOperation.Success(t.Entity),
            QueryResult.QueryFailedResult f => ApiOperation.Fail(f.ErrorCode, f.ErrorMessage, f.Exception),
            _ => throw new NotSupportedException()
        };
    }

    public static async Task<
        ApiOperationResult<ApiOperation.ApiFailedOperation, ApiOperation.ApiSuccessfulOperation>
    > PublishEvent(
        Func<string> eventContent,
        string category,
        string queue,
        IQueueService queueService,
        CancellationToken token
    )
    {
        var publishEventOperation = await queueService.PublishAsync(
            category,
            token,
            (queue, eventContent)
        );

        if (publishEventOperation is QueueOperation.FailedOperation f)
        {
            return ApiOperation.Fail(f.Error.Code, f.Error.Message, f.Error.ToException());
        }

        return ApiOperation.Success();
    }
}
