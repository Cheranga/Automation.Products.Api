using System.Diagnostics.CodeAnalysis;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

[ExcludeFromCodeCoverage]
public record struct LocationChangedEvent(
    string ProductId,
    string PreviousLocationCode,
    string CurrentLocationCode
);
