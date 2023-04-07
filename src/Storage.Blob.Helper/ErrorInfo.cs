namespace Storage.Blob.Helper;

public record struct ErrorInfo
{
    public Exception? Exception { get; }
    public int ErrorCode { get; }
    public string ErrorMessage { get; }

    private ErrorInfo(int errorCode, string errorMessage, Exception? exception)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    public static ErrorInfo New(Exception exception) => new(-1, "error", exception);
    public static ErrorInfo New(int errorCode, string errorMessage) =>
        new(errorCode, errorMessage, null);

    public static ErrorInfo New(int errorCode, string errorMessage, Exception? exception) =>
        new(errorCode, errorMessage, exception);
}