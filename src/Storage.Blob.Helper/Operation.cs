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

public record struct Operation<T>
{
    public T Data { get; init; }
    public ErrorInfo Error { get; init; }

    public bool IsFail => Error.ErrorCode > 0 || !string.IsNullOrEmpty(Error.ErrorMessage);

    public static Operation<T> Success(T data) => new() { Data = data };

    public static Operation<T> Failure(ErrorInfo errorInfo) => new() { Error = errorInfo };

    public static Operation<T> Failure(int errorCode, string errorMessage) =>
        new() { Error = ErrorInfo.New(errorCode, errorMessage) };

    public static Operation<T> Failure(int errorCode, string errorMessage, Exception exception) =>
        new() { Error = ErrorInfo.New(errorCode, errorMessage, exception) };
}
