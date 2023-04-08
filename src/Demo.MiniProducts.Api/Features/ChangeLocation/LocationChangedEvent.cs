namespace Demo.MiniProducts.Api.Features.ChangeLocation;

public record LocationChangedEvent(
    string ProductId,
    string PreviousLocationCode,
    string CurrentLocationCode
);
