using System.Diagnostics.CodeAnalysis;

namespace Storage.Queue.Helper;

[ExcludeFromCodeCoverage]
public static class ErrorCodes
{
    public const int UnregisteredQueueService = 500;
    public const int QueueUnavailable = 501;
    public const int PublishMessageError = 502;
    public const int PublishFailResponse = 503;
    public const int InvalidMessagePublishSettings = 504;
    public const int PeekError = 505;
    public const int ReadError = 506;
    public const int EmptyQueue = 507;
}

[ExcludeFromCodeCoverage]
public static class ErrorMessages
{
    public const string UnregisteredQueueService =
        "queue service is unregistered for the storage account.";

    public const string QueueUnavailable = "queue does not exist in the storage account.";
    public const string PublishMessageError = "error occurred when publishing message to the queue";

    public const string InvalidMessagePublishSettings = "invalid settings to publish message";
    public const string EmptyQueue = "queue is empty";
}