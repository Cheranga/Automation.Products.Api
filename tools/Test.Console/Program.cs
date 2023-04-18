using System.Text.Json;
using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using Funky.Azure.DataTable.Extensions.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Storage.Queue.Helper;
using Test.Console;
using Queues = Storage.Queue.Helper.Bootstrapper;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.RegisterMessagingWithConnectionString("test", "UseDevelopmentStorage=true");
        services.RegisterTablesWithConnectionString("test", "UseDevelopmentStorage=true");
    })
    .Build();

await DoQueues(host);

static async Task ReadMessageBatch(IHost host)
{
    var qs = host.Services.GetRequiredService<IQueueService>();
    var op = await qs.ReadBatchAsync(
        "test",
        new CancellationToken(),
        100,
        ("registrations", x => JsonSerializer.Deserialize<ProductRegisteredEvent>(x), 30)
    );
    Console.WriteLine(
        op switch
        {
            QueueOperation.SuccessOperation<List<ProductRegisteredEvent>> so
                => $"successfully read {so.Data.Count} messages",
            QueueOperation.FailedOperation _ => "failed reading batch messages",
            _ => "unsupported"
        }
    );
}

static async Task PublishMessageBatch(IHost host)
{
    var qs = host.Services.GetRequiredService<IQueueService>();
    var events = Enumerable
        .Range(1, 1000)
        .Select(x =>
        {
            var func = () =>
                JsonSerializer.Serialize(
                    new ProductRegisteredEvent(x.ToString(), "Tech", DateTime.UtcNow)
                );
            return func;
        });
    var publishOp = await qs.PublishBatchAsync(
        "test",
        new CancellationToken(),
        ("registrations", events)
    );

    Console.WriteLine(
        publishOp switch
        {
            QueueOperation.SuccessOperation _ => "successfully published as a batch",
            QueueOperation.FailedOperation _ => "failed batch",
            _ => "unsupported"
        }
    );
}

static async Task PeekMessage(IHost host)
{
    var qs = host.Services.GetRequiredService<IQueueService>();
    var peeked = await qs.PeekAsync(
        "test",
        new CancellationToken(),
        ("registrations", x => JsonSerializer.Deserialize<ProductRegisteredEvent>(x))
    );

    Console.WriteLine(
        peeked switch
        {
            QueueOperation.SuccessOperation<ProductRegisteredEvent> x
                => $"Peeked message = {x.Data.Category} :: {x.Data.ProductId} :: {x.Data.RegisteredDateTime:O}",
            QueueOperation.FailedOperation _ => "failed",
            _ => "unsupported"
        }
    );
}

static async Task ReadMessage(IHost host)
{
    var qs = host.Services.GetRequiredService<IQueueService>();
    var @event = await qs.ReadAsync(
        "test",
        new CancellationToken(),
        ("registrations", x => JsonSerializer.Deserialize<ProductRegisteredEvent>(x), 30)
    );

    Console.WriteLine(
        @event switch
        {
            QueueOperation.SuccessOperation<ProductRegisteredEvent> x
                => $"{x.Data.Category} :: {x.Data.ProductId} :: {x.Data.RegisteredDateTime:O}",
            QueueOperation.FailedOperation _ => "failed",
            _ => "unsupported"
        }
    );
}

static async Task DoQueues(IHost host)
{
    var queueService = host.Services.GetRequiredService<IQueueService>();
    var @event = new ProductRegistered("Laptop", "Tech", DateTime.UtcNow);

    var op = await queueService.PublishAsync(
        "test",
        new CancellationToken(),
        ("registrations1", @event.ToStringFunc(), 5, 10)
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
