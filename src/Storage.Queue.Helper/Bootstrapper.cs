using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace Storage.Queue.Helper;

[ExcludeFromCodeCoverage]
public static class Bootstrapper
{
    public static void RegisterMessagingWithConnectionString(
        this IServiceCollection services,
        string name,
        string connectionString
    )
    {
        services.AddAzureClients(builder =>
        {
            builder
                .AddQueueServiceClient(connectionString)
                .ConfigureOptions(options => { options.MessageEncoding = QueueMessageEncoding.Base64; })
                .WithName(name);
        });

        services.AddSingleton<IQueueService, QueueService>();
    }

    public static void RegisterMessagingWithManagedIdentity(
        this IServiceCollection services,
        string name,
        string storageAccountUrl
    )
    {
        services.AddAzureClients(builder =>
        {
            builder
                .AddQueueServiceClient(new Uri(storageAccountUrl))
                .ConfigureOptions(options => { options.MessageEncoding = QueueMessageEncoding.Base64; })
                .WithCredential(new ManagedIdentityCredential())
                .WithName(name);
        });

        services.AddSingleton<IQueueService, QueueService>();
    }
}