using System.Text;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Azure;
using static Storage.Blob.Helper.FunkyExtensions;

namespace Storage.Blob.Helper;

public static class StorageBlobSchema
{
    public static Operation<BlobServiceClient> GetNamedClient(
        IAzureClientFactory<BlobServiceClient> factory,
        string category
    ) =>
        ToOperation(() => factory.CreateClient(category))
            .MapFail(err => ErrorInfo.New(500, "unregistered client", err.Exception));

    public static Operation<BlobContainerClient> GetContainerClient(
        BlobServiceClient client,
        string container
    ) =>
        from containerClient in ToOperation(() => client.GetBlobContainerClient(container))
            .MapFail(err => ErrorInfo.New(501, "cannot get blob container client", err.Exception))
        from op in ToOperation(
            () =>
                containerClient.Exists()
                    ? Operation<BlobContainerClient>.Success(containerClient)
                    : Operation<BlobContainerClient>.Failure(501, "blob container does not exist")
        ).Data
        select op;

    public static Operation<BinaryData> ToBinary(string content) =>
        ToOperation(() => new BinaryData(Encoding.UTF8.GetBytes(content)));

    public static Operation<BlobClient> GetBlobClient(
        BlobContainerClient containerClient,
        string fileName
    ) =>
        ToOperation(() => containerClient.GetBlobClient(fileName))
            .MapFail(err => ErrorInfo.New(502, "blob client cant be created", err.Exception));

    public static Operation<BlobOperation> Upload(
        BlobClient client,
        BinaryData data,
        CancellationToken token,
        bool overwrite = true
    ) =>
        from op in ToOperation(() => client.Upload(data, overwrite, token))
            .MapFail(err => ErrorInfo.New(503, "blob upload error", err.Exception))
        from result in ToOperation(
            () =>
                op.GetRawResponse().IsError
                    ? BlobOperation.Failure(500, op.GetRawResponse().ReasonPhrase)
                    : BlobOperation.Success()
        )
        select result;

   
}
