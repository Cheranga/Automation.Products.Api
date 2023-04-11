using System.Linq.Expressions;
using Azure.Data.Tables;

namespace Storage.Table.Helper;

public interface ITableService
{
    Task<TableOperation> UpsertAsync<T>(
        string category,
        string table,
        T data,
        bool createNew,
        CancellationToken token
    ) where T : ITableEntity;

    Task<TableOperation> GetEntityAsync<T>(
        string category,
        string table,
        string partitionKey,
        string rowKey,
        CancellationToken token
    ) where T : class, ITableEntity;

    Task<TableOperation> GetEntityListAsync<T>(
        string category,
        string table,
        Expression<Func<T, bool>> filter,
        CancellationToken token
    ) where T : class, ITableEntity;
}