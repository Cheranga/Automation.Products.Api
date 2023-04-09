using System.Diagnostics.CodeAnalysis;
using LanguageExt.Common;

namespace Storage.Table.Helper;

[ExcludeFromCodeCoverage]
public class TableOperationException : Exception
{
    public TableOperationException(Error error) : base(error.Message, error.ToException()) { }
}

[ExcludeFromCodeCoverage]
public record TableOperationError : Error
{
    private readonly TableOperationException _exception;

    private TableOperationError(Error error)
    {
        Code = error.Code;
        Message = error.Message;
        _exception = new TableOperationException(error);
    }

    public override int Code { get; }

    public override string Message { get; }
    public override bool IsExceptional => true;
    public override bool IsExpected => false;

    public override bool Is<E>() => _exception is E;

    public override ErrorException ToErrorException() =>
        ErrorException.New(Code, Message, ErrorException.New(_exception));

    public static TableOperationError New(
        int errorCode,
        string errorMessage,
        Exception? exception = null
    ) =>
        exception is null
            ? new TableOperationError(Error.New(errorCode, errorMessage))
            : new TableOperationError(Error.New(errorCode, errorMessage, exception!));
}