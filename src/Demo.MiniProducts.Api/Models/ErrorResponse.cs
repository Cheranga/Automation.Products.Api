namespace Demo.MiniProducts.Api.Models;

public abstract class ErrorResponse
{
    public int Code { get; init; }
    public string Message { get; init; } = string.Empty;

    private ErrorResponse() { }

    public sealed class ProductNotFoundResponse : ErrorResponse
    {
        public int ProductId { get; init; }

        public static ProductNotFoundResponse New(int productId, int code, string message) =>
            new()
            {
                ProductId = productId,
                Code = code,
                Message = message
            };
    }

    public static ErrorResponse ProductNotFound(
        int productId,
        string message = "product not found"
    ) => ProductNotFoundResponse.New(productId, 404, message);
}
