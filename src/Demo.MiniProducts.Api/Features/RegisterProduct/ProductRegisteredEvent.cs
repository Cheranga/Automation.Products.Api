namespace Demo.MiniProducts.Api.Features.RegisterProduct;

public record struct ProductRegisteredEvent(string ProductId, string Category, DateTime RegisteredDateTime);