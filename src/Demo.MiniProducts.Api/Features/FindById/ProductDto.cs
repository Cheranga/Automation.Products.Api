namespace Demo.MiniProducts.Api.Features.FindById;

public record ProductDto(string Id, string Name, string Location, string Category);

public record ProductResponse(ProductDto Data);
