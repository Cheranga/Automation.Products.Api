using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Storage.Queue.Helper;
using Test.Console;
using Queues = Storage.Queue.Helper.Bootstrapper;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.RegisterWithConnectionString("test", "UseDevelopmentStorage=true");
    })
    .Build();

await DoQueues(host);

static async Task DoQueues(IHost host)
{
    var queueService = host.Services.GetRequiredService<IQueueService>();
    var @event = new ProductRegistered("Laptop", "Tech", DateTime.UtcNow);

    var op = await queueService.PublishAsync(
        "test",
        new CancellationToken(),
        ("registrations", () => JsonSerializer.Serialize(@event), 5, 10)
    );

    Console.WriteLine(
        op switch
        {
            QueueOperation.SuccessOperation => "successfully published",
            QueueOperation.FailedOperation f
                => $"ErrorCode:{f.ErrorCode}, ErrorMessage:{f.ErrorMessage}, Exception:{f.Exception}",
            _ => "unsupported operation"
        }
    );
}
