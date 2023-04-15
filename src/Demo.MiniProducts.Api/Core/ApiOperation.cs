using FluentValidation.Results;

namespace Demo.MiniProducts.Api.Core;

public abstract class ApiOperation
{
    public static ApiSuccessfulOperation<T> Success<T>(T data) =>
        ApiSuccessfulOperation<T>.New(data);

    public static ApiSuccessfulOperation Success() => ApiSuccessfulOperation.New();

    public static ApiFailedOperation Fail(
        int errorCode,
        string errorMessage,
        Exception? exception = null
    ) => ApiFailedOperation.New(errorCode, errorMessage, exception);

    public static ApiValidationFailureOperation FailedValidation(
        ValidationResult validationResult
    ) => ApiValidationFailureOperation.New(validationResult);

    public sealed class ApiSuccessfulOperation<T> : ApiOperation
    {
        public T Data { get; }

        private ApiSuccessfulOperation(T data)
        {
            Data = data;
        }

        public static ApiSuccessfulOperation<T> New(T data) => new(data);
    }

    public sealed class ApiSuccessfulOperation : ApiOperation
    {
        private ApiSuccessfulOperation() { }

        public static ApiSuccessfulOperation New() => new();
    }

    public sealed class ApiFailedOperation : ApiOperation
    {
        public int ErrorCode { get; }
        public string ErrorMessage { get; }
        public Exception? Exception { get; }

        private ApiFailedOperation(int errorCode, string errorMessage, Exception? exception)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        public static ApiFailedOperation New(
            int errorCode,
            string errorMessage,
            Exception? exception = null
        ) => new(errorCode, errorMessage, exception);
    }

    public sealed class ApiValidationFailureOperation : ApiOperation
    {
        public ValidationResult ValidationResult { get; }

        private ApiValidationFailureOperation(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }

        public static ApiValidationFailureOperation New(ValidationResult result) => new(result);
    }
}

public class ApiOperationResult<TA, TB>
    where TA : ApiOperation
    where TB : ApiOperation
{
    public ApiOperation Operation { get; }

    private ApiOperationResult(ApiOperation operation)
    {
        Operation = operation;
    }

    public static implicit operator ApiOperationResult<TA, TB>(TA a) => new(a);

    public static implicit operator ApiOperationResult<TA, TB>(TB b) => new(b);
}

public class ApiOperationResult<TA, TB, TC>
    where TA : ApiOperation
    where TB : ApiOperation
    where TC : ApiOperation
{
    public ApiOperation Operation { get; }

    private ApiOperationResult(ApiOperation operation)
    {
        Operation = operation;
    }

    public static implicit operator ApiOperationResult<TA, TB, TC>(TA a) => new(a);

    public static implicit operator ApiOperationResult<TA, TB, TC>(TB b) => new(b);

    public static implicit operator ApiOperationResult<TA, TB, TC>(TC c) => new(c);
}
