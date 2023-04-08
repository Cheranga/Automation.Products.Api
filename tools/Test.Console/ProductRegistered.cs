using Azure;
using Azure.Data.Tables;

namespace Test.Console;

public record ProductRegistered(string ProductId, string Category, DateTime RegisteredDateTime);

public record struct StudentEntity : ITableEntity
{
    public string Category { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public static StudentEntity New(string category, string id, string name) =>
        new()
        {
            Category = category,
            Id = id,
            Name = name,
            PartitionKey = category,
            RowKey = id
        };
}
