using System.Diagnostics.CodeAnalysis;

namespace Demo.MiniProducts.Api.Features.RegisterProduct;

[ExcludeFromCodeCoverage]
public record RegisterProductRequest(
    string ProductId,
    string Name,
    string LocationCode,
    string Category
);
