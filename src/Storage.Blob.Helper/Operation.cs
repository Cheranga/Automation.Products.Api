namespace Storage.Blob.Helper;

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