using Azure.Storage.Queues;
using LanguageExt;
using Microsoft.Extensions.Azure;
using static Storage.Queue.Helper.AzureQueueStorageWrapper;

namespace Storage.Queue.Helper;

public interface IQueueService
{
    Task<QueueOperation> PublishAsync(
        string category,
        CancellationToken token,
        (string queue, Func<string> content) messageInfo
    );

    Task<QueueOperation> PublishAsync(
        string category,
        CancellationToken token,
        (string queue, Func<string> content, int visibilitySeconds, int timeToLiveSeconds) messageInfo
    );
}

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
        ).Match(op => op, err => QueueOperation.Failure(err.Code, err.Message));

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
        ).Match(op => op, err => QueueOperation.Failure(err.Code, err.Message, err.ToException()));
}
