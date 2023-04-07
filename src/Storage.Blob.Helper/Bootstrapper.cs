using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace Storage.Blob.Helper;

public static class Bootstrapper
{
    public static void RegisterWithManagedIdentity(
        this IServiceCollection services,
        string name,
        string storageAccountUrl
    )
    {
        services.AddAzureClients(builder =>
        {
            builder
                .AddBlobServiceClient(new Uri(storageAccountUrl))
                .WithCredential(new ManagedIdentityCredential())
                .WithName(name);
        });

        services.AddSingleton<IBlobService, BlobService>();
    }

    public static void RegisterWithConnectionString(
        this IServiceCollection services,
        string name,
        string connectionString
    )
    {
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(connectionString).WithName(name);
        });
        
        services.AddSingleton<IBlobService, BlobService>();
    }
}
