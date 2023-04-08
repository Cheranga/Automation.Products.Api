using System.Diagnostics.CodeAnalysis;

namespace Storage.Table.Helper;


[ExcludeFromCodeCoverage]
public abstract class TableOperation
{
    private TableOperation()
    {
    }

    public static TableOperation Success() => new SuccessOperation();
    public static TableOperation Success<T>(T data) => new SuccessOperation<T>(data);

    public static TableOperation Failure(TableOperationError error) =>
        FailedOperation.New(error);

    public sealed class SuccessOperation : TableOperation
    {
    }

    public sealed class SuccessOperation<T> : TableOperation
    {
        public T Data { get; }

        public SuccessOperation(T data)
        {
            Data = data;
        }
    }

    public sealed class FailedOperation : TableOperation
    {
        private FailedOperation(TableOperationError error) => Error = error;

        public TableOperationError Error { get; }

        public static FailedOperation New(TableOperationError error) =>
            new(error);
    }
}