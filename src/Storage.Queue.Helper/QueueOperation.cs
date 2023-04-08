using System.Diagnostics.CodeAnalysis;

namespace Storage.Queue.Helper;

[ExcludeFromCodeCoverage]
public abstract class QueueOperation
{
    private QueueOperation()
    {
    }

    public static QueueOperation Success() => new SuccessOperation();

    public static QueueOperation Failure(QueueOperationError error) =>
        FailedOperation.New(error);

    public sealed class SuccessOperation : QueueOperation
    {
    }

    public sealed class FailedOperation : QueueOperation
    {
        private FailedOperation(QueueOperationError error) => Error = error;

        public QueueOperationError Error { get; }

        public static FailedOperation New(QueueOperationError error) =>
            new(error);
    }
}