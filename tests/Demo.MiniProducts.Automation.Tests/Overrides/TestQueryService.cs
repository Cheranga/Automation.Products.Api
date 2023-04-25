using System.Linq.Expressions;
using Azure.Data.Tables;
using Funky.Azure.DataTable.Extensions.Queries;
using LanguageExt.Common;

namespace Demo.MiniProducts.Automation.Tests.Overrides;

public class TestQueryService : IQueryService
{
    private readonly List<object> _records;

    public TestQueryService(List<object> records)
    {
        _records = records;
    }

    public Task<
        QueryResponse<
            QueryResult.QueryFailedResult,
            QueryResult.EmptyResult,
            QueryResult.SingleResult<T>
        >
    > GetEntityAsync<T>(
        string category,
        string table,
        string partitionKey,
        string rowKey,
        CancellationToken token
    ) where T : class, ITableEntity
    {
        QueryResponse<
            QueryResult.QueryFailedResult,
            QueryResult.EmptyResult,
            QueryResult.SingleResult<T>
        > response;
        var record =
            _records.FirstOrDefault(
                x =>
                    x is T model
                    && string.Equals(partitionKey, model.PartitionKey)
                    && string.Equals(rowKey, model.RowKey)
            ) as T;
        if (record == null)
        {
            response = QueryResult.Empty();
            return Task.FromResult(response);
        }

        response = QueryResult.Single(record);
        return Task.FromResult(response);
    }

    public Task<
        QueryResponse<
            QueryResult.QueryFailedResult,
            QueryResult.EmptyResult,
            QueryResult.SingleResult<T>,
            QueryResult.CollectionResult<T>
        >
    > GetEntityListAsync<T>(
        string category,
        string table,
        Expression<Func<T, bool>> filter,
        CancellationToken token
    ) where T : class, ITableEntity
    {
        QueryResponse<
            QueryResult.QueryFailedResult,
            QueryResult.EmptyResult,
            QueryResult.SingleResult<T>,
            QueryResult.CollectionResult<T>
        > response;

        if (_records is not List<T> { } typedRecords)
        {
            response = QueryResult.Fail(Error.New("incorrect data"));
            return Task.FromResult(response);
        }

        response = typedRecords.Count switch
        {
            0 => QueryResult.Empty(),
            1 => QueryResult.Single(typedRecords.First()),
            _ => QueryResult.Collection(typedRecords)
        };

        return Task.FromResult(response);
    }
}
