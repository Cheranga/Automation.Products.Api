using Azure.Data.Tables;
using Microsoft.Extensions.Azure;

namespace Storage.Table.Helper;

public interface ITableService
{
    Task UpsertAsync<T>(string category, string table, T data, CancellationToken token)
        where T : ITableEntity;
}

internal class TableService : ITableService
{
    private readonly IAzureClientFactory<TableServiceClient> _factory;

    public TableService(IAzureClientFactory<TableServiceClient> factory) => _factory = factory;

    public async Task UpsertAsync<T>(string category, string table, T data, CancellationToken token)
        where T : ITableEntity
    {
        var serviceClient = _factory.CreateClient(category);
        var tableClient = serviceClient.GetTableClient(table);
        var operation = await tableClient.UpsertEntityAsync(data, TableUpdateMode.Merge, token);
        if (operation.IsError) throw new Exception("error");
    }
}