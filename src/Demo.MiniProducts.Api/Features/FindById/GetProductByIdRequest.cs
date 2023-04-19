using System.Diagnostics.CodeAnalysis;

namespace Demo.MiniProducts.Api.Features.FindById;

[ExcludeFromCodeCoverage]
public class GetProductByIdRequest
{
    public GetProductByIdRequest(string category, string id)
    {
        Category = category;
        Id = id;
    }

    public string Category { get; set; }

    public string Id { get; set; }
}
