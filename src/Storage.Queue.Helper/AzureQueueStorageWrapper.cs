using System.Diagnostics.CodeAnalysis;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using LanguageExt;
using LanguageExt.Common;
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
                Error.New(ErrorCodes.QueueUnavailable, ErrorMessages.QueueUnavailable)
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
            from _1 in guard(
                    visibilitySeconds >= 0
                        && timeToLiveSeconds >= 0
                        && timeToLiveSeconds >= visibilitySeconds,
                    Error.New(
                        ErrorCodes.InvalidMessagePublishSettings,
                        ErrorMessages.InvalidMessagePublishSettings
                    )
                )
                .ToEff()
            from op in AffMaybe<Response<SendReceipt>>(
                async () =>
                    await queueClient.SendMessageAsync(
                        content(),
                        TimeSpan.FromSeconds(visibilitySeconds),
                        TimeSpan.FromSeconds(timeToLiveSeconds),
                        token
                    )
            )
            from _2 in guardnot(
                op.GetRawResponse().IsError,
                Error.New(ErrorCodes.PublishFailResponse, op.GetRawResponse().ReasonPhrase)
            )
            select op
        ).Match(
            _ => QueueOperation.Success(),
            err =>
                QueueOperation.Failure(
                    QueueOperationError.New(
                        ErrorCodes.PublishMessageError,
                        ErrorMessages.PublishMessageError,
                        err.ToException()
                    )
                )
        );

    public static Aff<QueueOperation> Publish(
        QueueClient queueClient,
        Func<string> content,
        CancellationToken token
    ) =>
        (
            from op in AffMaybe<Response<SendReceipt>>(
                async () => await queueClient.SendMessageAsync(content(), token)
            )
            from _ in guardnot(
                op.GetRawResponse().IsError,
                Error.New(ErrorCodes.PublishFailResponse, op.GetRawResponse().ReasonPhrase)
            )
            select op
        ).Match(
            _ => QueueOperation.Success(),
            err =>
                QueueOperation.Failure(
                    QueueOperationError.New(
                        ErrorCodes.PublishMessageError,
                        ErrorMessages.PublishMessageError,
                        err.ToException()
                    )
                )
        );
}
