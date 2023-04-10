using Azure.Storage.Queues;
using LanguageExt;
using Microsoft.Extensions.Azure;
using static Storage.Queue.Helper.AzureQueueStorageWrapper;

namespace Storage.Queue.Helper;

internal class QueueService : IQueueService
{
    private readonly IAzureClientFactory<QueueServiceClient> _factory;

    public QueueService(IAzureClientFactory<QueueServiceClient> factory) => _factory = factory;

    public async Task<QueueOperation> PublishAsync(
        string category,
        CancellationToken token,
        (string queue, Func<string> content) messageInfo
    ) =>
        (
            await (
                from sc in GetServiceClient(_factory, category)
                from qc in GetQueueClient(sc, messageInfo.queue)
                from op in Publish(qc, messageInfo.content, token)
                select op
            ).Run()
        ).Match(
            op => op,
            err =>
                QueueOperation.Failure(
                    QueueOperationError.New(err.Code, err.Message, err.ToException())
                )
        );

    public async Task<QueueOperation> PublishAsync(
        string category,
        CancellationToken token,
        (string queue, Func<string> content, int visibilitySeconds, int timeToLiveSeconds) messageInfo
    ) =>
        (
            await (
                from sc in GetServiceClient(_factory, category)
                from qc in GetQueueClient(sc, messageInfo.queue)
                from op in Publish(
                    qc,
                    messageInfo.content,
                    messageInfo.visibilitySeconds,
                    messageInfo.timeToLiveSeconds,
                    token
                )
                select op
            ).Run()
        ).Match(
            op => op,
            err =>
                QueueOperation.Failure(
                    QueueOperationError.New(err.Code, err.Message, err.ToException())
                )
        );

    public async Task<QueueOperation> PublishBatchAsync(
        string category,
        CancellationToken token,
        (string queue, IEnumerable<
            Func<string>
        > contentFuncs) messageInfo
    ) =>
        (
            await (
                from sc in GetServiceClient(_factory, category)
                from qc in GetQueueClient(sc, messageInfo.queue)
                from op in PublishBatch(qc, token, messageInfo.contentFuncs)
                select op
            ).Run()
        ).Match(
            op => op,
            err =>
                QueueOperation.Failure(
                    QueueOperationError.New(err.Code, err.Message, err.ToException())
                )
        );

    public async Task<QueueOperation> PeekAsync<T>(
        string category,
        CancellationToken token,
        (string queue, Func<string, T> jsonToModel) messageInfo
    ) =>
        (
            await (
                from sc in GetServiceClient(_factory, category)
                from qc in GetQueueClient(sc, messageInfo.queue)
                from op in Peek<T>(qc, messageInfo.jsonToModel, token)
                select op
            ).Run()
        ).Match(
            x => x,
            err =>
                QueueOperation.Failure(
                    QueueOperationError.New(err.Code, err.Message, err.ToException())
                )
        );

    public async Task<QueueOperation> ReadAsync<T>(
        string category,
        CancellationToken token,
        (string queue, Func<string, T> jsonToModel, int visibilityInSeconds) messageInfo
    ) =>
        (
            await (
                from sc in GetServiceClient(_factory, category)
                from qc in GetQueueClient(sc, messageInfo.queue)
                from op in Read<T>(
                    qc,
                    messageInfo.jsonToModel,
                    messageInfo.visibilityInSeconds,
                    token
                )
                select op
            ).Run()
        ).Match(
            x => x,
            err =>
                QueueOperation.Failure(
                    QueueOperationError.New(err.Code, err.Message, err.ToException())
                )
        );
}
