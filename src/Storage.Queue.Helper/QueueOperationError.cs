using System.Diagnostics.CodeAnalysis;
using LanguageExt.Common;

namespace Storage.Queue.Helper;

[ExcludeFromCodeCoverage]
public class QueueOperationException : Exception
{
    public QueueOperationException(Error error) : base(error.Message, error.ToException()) { }
}

[ExcludeFromCodeCoverage]
public record QueueOperationError : Error
{
    private readonly QueueOperationException _exception;

    private QueueOperationError(Error error)
    {
        Code = error.Code;
        Message = error.Message;
        _exception = new QueueOperationException(error);
    }

    public override int Code { get; }

    public override string Message { get; }
    public override bool IsExceptional => true;
    public override bool IsExpected => false;

    public override bool Is<E>() => _exception is E;

    public override ErrorException ToErrorException() =>
        ErrorException.New(Code, Message, ErrorException.New(_exception));

    public static QueueOperationError New(
        int errorCode,
        string errorMessage,
        Exception? exception = null
    ) =>
        exception is null
            ? new QueueOperationError(Error.New(errorCode, errorMessage))
            : new QueueOperationError(Error.New(errorCode, errorMessage, exception!));
}
