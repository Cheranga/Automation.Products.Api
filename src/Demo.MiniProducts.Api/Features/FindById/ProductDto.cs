using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Demo.MiniProducts.Api.Core;
using Swashbuckle.AspNetCore.Filters;

namespace Demo.MiniProducts.Api.Features.FindById;

[ExcludeFromCodeCoverage]
public class ProductDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Location { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public class ProductResponse : IDtoRequest<ProductResponse>, IExamplesProvider<ProductResponse>
{
    [Required]
    public ProductDto Data { get; set; } = null!;

    public ProductResponse GetExamples() =>
        new()
        {
            Data = new ProductDto
            {
                Category = "TECH",
                Id = "PROD1",
                Name = "Laptop",
                Location = "1234"
            }
        };
}
