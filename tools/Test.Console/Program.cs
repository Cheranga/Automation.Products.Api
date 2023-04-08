using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Storage.Queue.Helper;
using Storage.Table.Helper;
using Test.Console;
using Queues = Storage.Queue.Helper.Bootstrapper;
using Tables = Storage.Table.Helper.Bootstrapper;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        Queues.RegisterMessagingWithConnectionString(services, "test", "UseDevelopmentStorage=true");
        Tables.RegisterTablesWithConnectionString(services, "students", "UseDevelopmentStorage=true");
    })
    .Build();

//await DoQueues(host);
await DoTables(host);

static async Task DoQueues(IHost host)
{
    var queueService = host.Services.GetRequiredService<IQueueService>();
    var @event = new ProductRegistered("Laptop", "Tech", DateTime.UtcNow);

    var op = await queueService.PublishAsync(
        "test",
        new CancellationToken(),
        ("registrations1", () => JsonSerializer.Serialize(@event), 5, 10)
    );

    Console.WriteLine(
        op switch
        {
            QueueOperation.SuccessOperation => "successfully published",
            QueueOperation.FailedOperation f
                => $"ErrorCode:{f.Error.Code}, ErrorMessage:{f.Error.Message}, Exception:{f.Error.Exception}",
            _ => "unsupported operation"
        }
    );
}

static async Task DoTables(IHost host)
{
    var tableService = host.Services.GetRequiredService<ITableService>();
    await tableService.UpsertAsync(
        "students",
        "techstudents",
        StudentEntity.New("IT", "1", "Cheranga"),
        true,
        new CancellationToken()
    );
}
