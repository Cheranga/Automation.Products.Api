namespace Demo.MiniProducts.Api.Features.ChangeLocation;

public record struct LocationChangedEvent(
    string ProductId,
    string PreviousLocationCode,
    string CurrentLocationCode
);
