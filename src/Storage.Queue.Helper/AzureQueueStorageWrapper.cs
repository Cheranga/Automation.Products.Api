using System.Diagnostics.CodeAnalysis;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using LanguageExt;
using Microsoft.Extensions.Azure;
using static LanguageExt.Prelude;

namespace Storage.Queue.Helper;

[ExcludeFromCodeCoverage]
internal static class AzureQueueStorageWrapper
{
    public static Eff<QueueServiceClient> GetServiceClient(
        IAzureClientFactory<QueueServiceClient> factory,
        string category
    ) =>
        EffMaybe<QueueServiceClient>(() => factory.CreateClient(category))
            .MapFail(
                ex =>
                    QueueOperationError.New(
                        ErrorCodes.UnregisteredQueueService,
                        ErrorMessages.UnregisteredQueueService,
                        ex
                    )
            );

    public static Eff<QueueClient> GetQueueClient(QueueServiceClient serviceClient, string queue) =>
        (
            from qc in EffMaybe<QueueClient>(() => serviceClient.GetQueueClient(queue))
            from _ in guard(
                qc.Exists(),
                QueueOperationError.New(ErrorCodes.QueueUnavailable, ErrorMessages.QueueUnavailable)
            )
            select qc
        ).MapFail(
            ex =>
                QueueOperationError.New(
                    ErrorCodes.QueueUnavailable,
                    ErrorMessages.QueueUnavailable,
                    ex
                )
        );

    public static Aff<QueueOperation> Publish(
        QueueClient queueClient,
        Func<string> content,
        int visibilitySeconds,
        int timeToLiveSeconds,
        CancellationToken token
    ) =>
        (
            from op in AffMaybe<Response<SendReceipt>>(
                async () =>
                    await queueClient.SendMessageAsync(
                        content(),
                        TimeSpan.FromSeconds(visibilitySeconds),
                        TimeSpan.FromSeconds(timeToLiveSeconds),
                        token
                    )
            )
            from a in guardnot(
                op.GetRawResponse().IsError,
                QueueOperationError.New(
                    ErrorCodes.PublishFailResponse,
                    op.GetRawResponse().ReasonPhrase
                )
            )
            select op
        ).Match(
            _ => QueueOperation.Success(),
            _ =>
                QueueOperation.Failure(
                    ErrorCodes.PublishMessageError,
                    ErrorMessages.PublishMessageError
                )
        );

    public static Aff<QueueOperation> Publish(
        QueueClient queueClient,
        Func<string> content,
        CancellationToken token
    ) => Publish(queueClient, content, 0, 0, token);
}
