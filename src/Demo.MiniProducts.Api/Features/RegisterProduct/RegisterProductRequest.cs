using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Demo.MiniProducts.Api.Core;
using Swashbuckle.AspNetCore.Filters;

namespace Demo.MiniProducts.Api.Features.RegisterProduct;

/// <summary>
///     The request to register a product
/// </summary>
[ExcludeFromCodeCoverage]
public record RegisterProductRequest
    : IDto<RegisterProductRequest>,
        IExamplesProvider<RegisterProductRequest>
{
    public RegisterProductRequest(
        string productId,
        string name,
        string locationCode,
        string category
    )
    {
        ProductId = productId;
        Name = name;
        LocationCode = locationCode;
        Category = category;
    }

    public RegisterProductRequest() : this(string.Empty, string.Empty, string.Empty, string.Empty)
    { }

    
    [Required]
    [DisplayName("The product id.")]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    public string Name { get;  set;} = string.Empty;

    [Required]
    public string LocationCode { get; set; } = string.Empty;

    [Required]
    public string Category { get;  set;} = string.Empty;


    public RegisterProductRequest GetExamples() => new("PROD1", "Laptop", "6666", "TECH");
}
