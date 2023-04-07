namespace Storage.Blob.Helper;

public abstract class BlobOperation
{
    private BlobOperation() { }

    public sealed class SuccessOperation : BlobOperation { }

    public sealed class FailedOperation : BlobOperation
    {
        public int ErrorCode { get; }
        public string ErrorMessage { get; }

        public FailedOperation(int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }

    public static BlobOperation Success() => new SuccessOperation();

    public static BlobOperation Failure(int errorCode, string errorMessage) =>
        new FailedOperation(errorCode, errorMessage);
}