using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using static Storage.Blob.Helper.StorageBlobSchema;

namespace Storage.Blob.Helper;

public interface IBlobService
{
    Task<BlobOperation> UploadAsync(
        (string container, string fileName, string fileContent, bool overwrite) blobInfo,
        string category,
        CancellationToken token
    );

    Operation<BlobOperation> FunkyUploadAsync(
        (string container, string fileName, string fileContent, bool overwrite) blobInfo,
        string category,
        CancellationToken token
    );
}

internal class BlobService : IBlobService
{
    private readonly IAzureClientFactory<BlobServiceClient> _factory;

    public BlobService(IAzureClientFactory<BlobServiceClient> factory) => _factory = factory;

    public async Task<BlobOperation> UploadAsync(
        (string container, string fileName, string fileContent, bool overwrite) blobInfo,
        string category,
        CancellationToken token
    )
    {
        var client = _factory.CreateClient(category);
        var containerClient = client.GetBlobContainerClient(blobInfo.container);
        var binaryData = new BinaryData(Encoding.UTF8.GetBytes(blobInfo.fileContent));
        var blobClient = containerClient.GetBlobClient(blobInfo.fileName);
        var operation = await blobClient.UploadAsync(binaryData, blobInfo.overwrite, token);
        if (operation.GetRawResponse().IsError)
            return BlobOperation.Failure(500, operation.GetRawResponse().ReasonPhrase);

        return BlobOperation.Success();
    }

    public Operation<BlobOperation> FunkyUploadAsync(
        (string container, string fileName, string fileContent, bool overwrite) blobInfo,
        string category,
        CancellationToken token
    ) =>
        from client in GetNamedClient(_factory, category)
        from cc in GetContainerClient(client, blobInfo.container)
        from bc in GetBlobClient(cc, blobInfo.fileName)
        from bd in ToBinary(blobInfo.fileContent)
        from op in Upload(bc, bd, token)
        select op;
}
