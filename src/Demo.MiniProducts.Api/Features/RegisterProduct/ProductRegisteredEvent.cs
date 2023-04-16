using System.Diagnostics.CodeAnalysis;

namespace Demo.MiniProducts.Api.Features.RegisterProduct;

[ExcludeFromCodeCoverage]
public record struct ProductRegisteredEvent(string ProductId, string Category, DateTime RegisteredDateTime);