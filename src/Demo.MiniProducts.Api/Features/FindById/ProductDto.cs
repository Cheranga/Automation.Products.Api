using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Demo.MiniProducts.Api.Core;
using Swashbuckle.AspNetCore.Filters;

namespace Demo.MiniProducts.Api.Features.FindById;

[ExcludeFromCodeCoverage]
public class ProductDto
{
    public ProductDto(string id, string name, string location, string category)
    {
        Id = id;
        Name = name;
        Location = location;
        Category = category;
    }

    [Required]
    public string Id { get; }

    [Required]
    public string Name { get; }

    [Required]
    public string Location { get; }

    [Required]
    public string Category { get; }
}

[ExcludeFromCodeCoverage]
public class ProductResponse : IDtoRequest<ProductResponse>, IExamplesProvider<ProductResponse>
{
    public ProductResponse(ProductDto data) => Data = data;

    public ProductResponse()
        : this(new ProductDto(string.Empty, string.Empty, string.Empty, string.Empty)) { }

    [Required]
    public ProductDto Data { get; }

    public ProductResponse GetExamples() => new(new ProductDto("PROD1", "Laptop", "1234", "TECH"));
}
