using System.Diagnostics.CodeAnalysis;

namespace Storage.Queue.Helper;

[ExcludeFromCodeCoverage]
public abstract class QueueOperation
{
    private QueueOperation()
    {
    }

    public static QueueOperation Success() => new SuccessOperation();

    public static QueueOperation Success<T>(T data) => new SuccessOperation<T>(data);

    public static QueueOperation Failure(QueueOperationError error) =>
        FailedOperation.New(error);

    public sealed class SuccessOperation<T> : QueueOperation
    {
        public T Data { get; }

        public SuccessOperation(T data)
        {
            Data = data;
        }
    }
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