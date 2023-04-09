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

    Task<QueueOperation> PeekAsync<T>(
        string category,
        CancellationToken token,
        (string queue, Func<string, T> jsonToModel) messageInfo
    );
    
    Task<QueueOperation> ReadAsync<T>(
        string category,
        CancellationToken token,
        (string queue, Func<string, T> jsonToModel, int visibilityInSeconds) messageInfo
    );
}