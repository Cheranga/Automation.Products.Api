using LanguageExt.Common;

namespace Demo.MiniProducts.Api.Core;

public record ApiError<T> : Error
{
    public T Data { get; }

    private ApiError(T data, string message)
    {
        Data = data;
        Message = string.IsNullOrEmpty(message)? "error": message;
    }

    public override bool Is<E>() => true;

    public override ErrorException ToErrorException() => ErrorException.New(Code, Message);
    public override string Message { get; }
    public override bool IsExceptional => true;
    public override bool IsExpected => false;

    public static Error New(T data, string message) => new ApiError<T>(data, message);
    public static Error New(T data) => New(data, string.Empty);
}