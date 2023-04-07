// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Storage.Blob.Helper;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.RegisterWithConnectionString("test", "UseDevelopmentStorage=true");
    })
    .Build();

var blobService = host.Services.GetRequiredService<IBlobService>();
var operation = await blobService.UploadAsync(
    ("testcontainer", "testfile.txt", "Cheranga Hatangala", true),
    "test",
    new CancellationToken()
);

var a = blobService.FunkyUploadAsync(("testcontainer", "AU/DATA/testfile.txt", "Cheranga Hatangala", true),
    "test",
    new CancellationToken());

PrintResults(operation);

static void PrintResults(BlobOperation operation)
{
    switch (operation)
    {
        case BlobOperation.SuccessOperation:
            Console.WriteLine("successful");
            return;
        case BlobOperation.FailedOperation f:
            Console.WriteLine($"{f.ErrorCode} with {f.ErrorMessage}");
            return;
        default:
            throw new NotSupportedException();
    }
}
