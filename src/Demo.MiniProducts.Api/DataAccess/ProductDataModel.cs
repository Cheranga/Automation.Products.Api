using System.Diagnostics;
using Azure;
using Azure.Data.Tables;

namespace Demo.MiniProducts.Api.DataAccess;

[DebuggerDisplay("{Category} - {ProductId} - {Name} - {LocationCode}")]
public class ProductDataModel : ITableEntity
{
    public string Category { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public static ProductDataModel New(string category, string productId, string name, string locationCode) =>
        new()
        {
            PartitionKey = category.ToUpper(),
            RowKey = productId.ToUpper(),
            Category = category,
            ProductId = productId,
            Name = name,
            LocationCode = locationCode
        };
}
