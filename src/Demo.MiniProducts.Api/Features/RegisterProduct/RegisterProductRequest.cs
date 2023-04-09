namespace Demo.MiniProducts.Api.Features.RegisterProduct;

public record RegisterProductRequest(
    string ProductId,
    string Name,
    string LocationCode,
    string Category
);
