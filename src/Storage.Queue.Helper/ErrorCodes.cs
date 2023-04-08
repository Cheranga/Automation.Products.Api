using System.Diagnostics.CodeAnalysis;

namespace Storage.Queue.Helper;

[ExcludeFromCodeCoverage]
public static class ErrorCodes
{
    public const int UnregisteredQueueService = 500;
    public const int QueueUnavailable = 501;
    public const int PublishMessageError = 502;
    public const int PublishFailResponse = 503;
}

[ExcludeFromCodeCoverage]
public static class ErrorMessages
{
    public const string UnregisteredQueueService =
        "queue service is unregistered for the storage account.";

    public const string QueueUnavailable = "queue does not exist in the storage account.";
    public const string PublishMessageError = "error occurred when publishing message to the queue";

    public const string PublishFailResponse =
        "publish to queue operation returned unsuccessful response";
}
