using System.Diagnostics.CodeAnalysis;

namespace Storage.Queue.Helper;

[ExcludeFromCodeCoverage]
public abstract class QueueOperation
{
    private QueueOperation() { }

    public sealed class SuccessOperation : QueueOperation { }

    public sealed class FailedOperation : QueueOperation
    {
        public int ErrorCode { get;  }
        public string ErrorMessage { get;  }
        public Exception? Exception { get; }

        private FailedOperation(int errorCode, string errorMessage, Exception? exception)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        public static FailedOperation New(int errorCode, string errorMessage) =>
            new(errorCode, errorMessage, null);
        
        public static FailedOperation New(int errorCode, string errorMessage, Exception? exception) =>
            new(errorCode, errorMessage, exception);
    }

    public static QueueOperation Success() => new SuccessOperation();

    public static QueueOperation Failure(int errorCode, string errorMessage) =>
        FailedOperation.New(errorCode, errorMessage);
    
    public static QueueOperation Failure(int errorCode, string errorMessage, Exception exception) =>
        FailedOperation.New(errorCode, errorMessage, exception);
}
