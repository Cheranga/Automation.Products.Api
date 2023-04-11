using System.Diagnostics.CodeAnalysis;
using Azure.Data.Tables;

namespace Storage.Table.Helper;

[ExcludeFromCodeCoverage]
public abstract class TableOperation
{
    private TableOperation() { }

    public static TableOperation Success() => CommandOperation.New();

    public static TableOperation GetEntity<T>(T data) where T : class, ITableEntity =>
        QuerySingleOperation<T>.New(data);

    public static TableOperation GetEntities<T>(IEnumerable<T> data)
        where T : class, ITableEntity => QueryListOperation<T>.New(data);

    public static TableOperation Failure(TableOperationError error) => FailedOperation.New(error);

    public sealed class CommandOperation : TableOperation
    {
        private CommandOperation() { }

        public static CommandOperation New() => new();
    }

    public sealed class QuerySingleOperation<T> : TableOperation where T : class, ITableEntity
    {
        private QuerySingleOperation(T entity) => Entity = entity;

        public T Entity { get; }

        public static QuerySingleOperation<T> New(T data) => new(data);
    }

    public sealed class QueryListOperation<T> : TableOperation where T : class, ITableEntity
    {
        private QueryListOperation(IEnumerable<T> entities) =>
            Entities = entities?.ToList() ?? new List<T>();

        public List<T> Entities { get; }

        public static QueryListOperation<T> New(IEnumerable<T> data) => new(data);
    }

    public sealed class FailedOperation : TableOperation
    {
        private FailedOperation(TableOperationError error) => Error = error;

        public TableOperationError Error { get; }

        public static FailedOperation New(TableOperationError error) => new(error);
    }
}
