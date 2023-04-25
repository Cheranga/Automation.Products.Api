using Azure.Data.Tables;
using Funky.Azure.DataTable.Extensions.Commands;
using LanguageExt.Common;

namespace Demo.MiniProducts.Automation.Tests.Overrides;

public class TestCommandService : ICommandService
{
    private readonly List<object> _records;

    public TestCommandService() => _records = new List<object>();

    public Task<
        CommandResponse<
            CommandOperation.CommandFailedOperation,
            CommandOperation.CommandSuccessOperation
        >
    > UpdateAsync<T>(string category, string table, T data, CancellationToken token)
        where T : class, ITableEntity
    {
        for (var index = 0; index < _records.Count; index++)
            if (
                _records[index] is T model
                && string.Equals(model.PartitionKey, data.PartitionKey)
                && string.Equals(model.RowKey, data.RowKey)
            )
            {
                _records[index] = model;
                return Task.FromResult<
                    CommandResponse<
                        CommandOperation.CommandFailedOperation,
                        CommandOperation.CommandSuccessOperation
                    >
                >(CommandOperation.Success());
            }

        return Task.FromResult<
            CommandResponse<
                CommandOperation.CommandFailedOperation,
                CommandOperation.CommandSuccessOperation
            >
        >(CommandOperation.Fail(Error.New("not found")));
    }

    public Task<
        CommandResponse<
            CommandOperation.CommandFailedOperation,
            CommandOperation.CommandSuccessOperation
        >
    > UpsertAsync<T>(string category, string table, T data, CancellationToken token)
        where T : class, ITableEntity =>
        Task.FromResult<
            CommandResponse<
                CommandOperation.CommandFailedOperation,
                CommandOperation.CommandSuccessOperation
            >
        >(CommandOperation.Success());
}
