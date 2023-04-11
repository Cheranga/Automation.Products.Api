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
    public TableOperationException Exception { get; }

    private TableOperationError(Error error)
    {
        Code = error.Code;
        Message = error.Message;
        Exception = new TableOperationException(error);
    }

    public override int Code { get; }

    public override string Message { get; }
    public override bool IsExceptional => true;
    public override bool IsExpected => false;

    public override bool Is<E>() => Exception is E;

    public override ErrorException ToErrorException() =>
        ErrorException.New(Code, Message, ErrorException.New(Exception));

    public static TableOperationError New(
        int errorCode,
        string errorMessage,
        Exception? exception = null
    ) =>
        exception is null
            ? new TableOperationError(Error.New(errorCode, errorMessage))
            : new TableOperationError(Error.New(errorCode, errorMessage, exception!));
}
