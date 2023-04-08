using Azure;
using Azure.Data.Tables;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Azure;
using static LanguageExt.Prelude;

namespace Storage.Table.Helper;

internal static class AzureTableStorageWrapper
{
    public static Eff<TableServiceClient> GetServiceClient(
        IAzureClientFactory<TableServiceClient> factory,
        string category
    ) =>
        EffMaybe<TableServiceClient>(() => factory.CreateClient(category))
            .MapFail(
                ex =>
                    TableOperationError.New(
                        ErrorCodes.UnregisteredTableService,
                        ErrorMessages.UnregisteredTableService,
                        ex
                    )
            );

    public static Eff<TableClient> GetTableClient(TableServiceClient serviceClient, string table) =>
        (
            from tc in EffMaybe<TableClient>(() => serviceClient.GetTableClient(table))
            select tc
        ).MapFail(
            ex =>
                TableOperationError.New(
                    ErrorCodes.TableUnavailable,
                    ErrorMessages.TableUnavailable,
                    ex
                )
        );

    public static Aff<TableOperation> Upsert<T>(
        TableClient client,
        T data,
        CancellationToken token,
        bool merge = true
    ) where T : ITableEntity =>
        (
            from op in AffMaybe<Response>(
                async () =>
                    await client.UpsertEntityAsync(
                        data,
                        merge ? TableUpdateMode.Merge : TableUpdateMode.Replace,
                        token
                    )
            )
            from _ in guardnot(
                op.IsError,
                Error.New(ErrorCodes.CannotUpsert, ErrorMessages.CannotUpsert)
            )
            select op
        ).Match(
            _ => TableOperation.Success(),
            err =>
                TableOperation.Failure(
                    TableOperationError.New(err.Code, err.Message, err.ToException())
                )
        );
}
