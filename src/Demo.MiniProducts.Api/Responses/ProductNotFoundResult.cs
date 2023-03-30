using Microsoft.AspNetCore.Mvc;

namespace Demo.MiniProducts.Api.Responses;

public class ProductNotFoundResult : IResult
{
    public Task ExecuteAsync(HttpContext httpContext) => throw new NotImplementedException();
}