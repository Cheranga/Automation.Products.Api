using System.Linq.Expressions;
using Azure.Data.Tables;
using Funky.Azure.DataTable.Extensions.Queries;

namespace Demo.MiniProducts.Automation.Tests;

public class TestQueryService : IQueryService
{
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
    ) where T : class, ITableEntity => throw new NotImplementedException();

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
    ) where T : class, ITableEntity => throw new NotImplementedException();
}
