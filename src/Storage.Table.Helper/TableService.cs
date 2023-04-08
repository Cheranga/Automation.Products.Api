using Azure.Data.Tables;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Azure;
using static LanguageExt.Prelude;
using static Storage.Table.Helper.AzureTableStorageWrapper;

namespace Storage.Table.Helper;

public interface ITableService
{
    Task<TableOperation> UpsertAsync<T>(
        string category,
        string table,
        T data,
        bool merge,
        CancellationToken token
    ) where T : ITableEntity;

    Task<TableOperation> GetAsync<T>(
        string category,
        string table,
        string partitionKey,
        string rowKey,
        CancellationToken token
    ) where T : class, ITableEntity;
}

internal class TableService : ITableService
{
    private readonly IAzureClientFactory<TableServiceClient> _factory;

    public TableService(IAzureClientFactory<TableServiceClient> factory) => _factory = factory;

    public async Task<TableOperation> UpsertAsync<T>(
        string category,
        string table,
        T data,
        bool merge,
        CancellationToken token
    ) where T : ITableEntity =>
        (
            await (
                from _1 in ValidateEmptyString(category)
                from _2 in ValidateEmptyString(table)
                from sc in GetServiceClient(_factory, category)
                from tc in GetTableClient(sc, table)
                from op in Upsert(tc, data, token, merge)
                select op
            ).Run()
        ).Match(
            op => op,
            err =>
                TableOperation.Failure(
                    TableOperationError.New(err.Code, err.Message, err.ToException())
                )
        );

    public async Task<TableOperation> GetAsync<T>(
        string category,
        string table,
        string partitionKey,
        string rowKey,
        CancellationToken token
    ) where T : class, ITableEntity =>
        (
            await (
                from _1 in ValidateEmptyString(category)
                from _2 in ValidateEmptyString(table)
                from _3 in ValidateEmptyString(partitionKey)
                from _4 in ValidateEmptyString(rowKey)
                from sc in GetServiceClient(_factory, category)
                from tc in GetTableClient(sc, table)
                from op in GetEntityAsync<T>(tc, partitionKey.ToUpper(), rowKey.ToUpper(), token)
                select op
            ).Run()
        ).Match(
            operation => operation,
            err =>
                TableOperation.Failure(
                    TableOperationError.New(err.Code, err.Message, err.ToException())
                )
        );

    private static Eff<Unit> ValidateEmptyString(string s) =>
        from _1 in guardnot(
                string.IsNullOrWhiteSpace(s),
                Error.New(ErrorCodes.Invalid, ErrorMessages.Invalid)
            )
            .ToEff()
        select unit;

    private static Eff<Unit> ValidateEmptyString(
        string category,
        string table,
        string partitionKey,
        string rowKey
    ) =>
        from _1 in guardnot(
                string.IsNullOrWhiteSpace(category),
                Error.New("category cannot be null or empty")
            )
            .ToEff()
        from _2 in guardnot(
            string.IsNullOrWhiteSpace(table),
            Error.New("table cannot be null or empty")
        )
        from _3 in guardnot(
            string.IsNullOrWhiteSpace(partitionKey),
            Error.New("partition key cannot be null or empty")
        )
        from _4 in guardnot(
            string.IsNullOrWhiteSpace(rowKey),
            Error.New("row key cannot be null or empty")
        )
        select unit;
}
