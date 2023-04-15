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
                from qc in QueueClient(_factory, category, messageInfo.queue)
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
                from qc in QueueClient(_factory, category, messageInfo.queue)
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
        (string queue, IEnumerable<Func<string>> contentFuncs) messageInfo
    ) =>
        (
            await (
                from qc in QueueClient(_factory, category, messageInfo.queue)
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
                from qc in QueueClient(_factory, category, messageInfo.queue)
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
                from qc in QueueClient(_factory, category, messageInfo.queue)
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

    public async Task<QueueOperation> ReadBatchAsync<T>(
        string category,
        CancellationToken token,
        int numberOfMessagesToRead,
        (string queue, Func<string, T> jsonToModel, int visibilityInSeconds) messageInfo
    ) =>
        (
            await (
                from qc in QueueClient(_factory, category, messageInfo.queue)
                from op in ReadBatch(qc, numberOfMessagesToRead, messageInfo.jsonToModel, token)
                select op
            ).Run()
        ).Match(
            messages => QueueOperation.Success(messages),
            err =>
                QueueOperation.Failure(
                    QueueOperationError.New(err.Code, err.Message, err.ToException())
                )
        );

    private static Aff<QueueClient> QueueClient(
        IAzureClientFactory<QueueServiceClient> factory,
        string category,
        string queue
    ) =>
        from sc in GetServiceClient(factory, category)
        from qc in GetQueueClient(sc, queue)
        select qc;
}
