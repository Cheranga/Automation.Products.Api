using System.Diagnostics.CodeAnalysis;

namespace Demo.MiniProducts.Api.Features.RegisterProduct;

[ExcludeFromCodeCoverage]
public record RegisterProductSettings(string Category, string Queue, string Table, string ConnectionString);