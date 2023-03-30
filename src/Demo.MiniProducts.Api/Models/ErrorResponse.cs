namespace Demo.MiniProducts.Api.Models;

public abstract class ErrorResponse
{
    public int Code { get; init; }
    public string Message { get; init; } = string.Empty;

    private ErrorResponse() { }

    public sealed class InvalidDataResponse : ErrorResponse { }

    public sealed class NotFoundResponse : ErrorResponse { }

    public static InvalidDataResponse InvalidData(string message = "invalid data") =>
        new() { Code = 400, Message = message };

    public static NotFoundResponse NotFound(string message = "not found") =>
        new() { Code = 404, Message = message };
}
