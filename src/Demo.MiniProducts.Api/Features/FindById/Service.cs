using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;
using ResponseExtensions = Demo.MiniProducts.Api.Extensions.ResponseExtensions;

namespace Demo.MiniProducts.Api.Features.FindById;

public record ProductDto(string Id, string Name, string Location, string Category, bool IsActive, bool IsOnPromotion);

public record ProductResponse(ProductDto Data) : ResponseDtoBase<ProductDto>(Data);

public static class Service
{
    public static async Task<Results<ProblemHttpResult, Ok<ProductResponse>>> GetProductDetailsById(
        [FromRoute] int Id,
        [FromServices] ProductsDbContext context
    ) =>
        await context.Products.FindAsync(Id) is { } product
            ? Ok(new ProductResponse(product.ToDto()))
            : ResponseExtensions.ProductUnfound(Id.ToString());
}