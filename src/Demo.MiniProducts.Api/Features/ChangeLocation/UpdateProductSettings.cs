using System.Diagnostics.CodeAnalysis;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

[ExcludeFromCodeCoverage]
public record UpdateProductSettings(string Category, string Queue, string ConnectionString);
