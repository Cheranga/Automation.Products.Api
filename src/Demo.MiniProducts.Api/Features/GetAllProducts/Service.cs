using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Features.FindById;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;
using ResponseExtensions = Demo.MiniProducts.Api.Extensions.ResponseExtensions;

namespace Demo.MiniProducts.Api.Features.GetAllProducts;

public record ProductListResponse(List<ProductDto> Data) : ResponseDtoBase<List<ProductDto>>(Data);

public static class Service
{
    public static async Task<Results<ProblemHttpResult, Ok<ProductListResponse>>> GetAllProducts(
        [FromServices] ProductsDbContext context
    ) =>
        await context.Products.ToListAsync() is { } products
            ? products.Any()
                ? Ok(new ProductListResponse(products.ToDto()))
                : ResponseExtensions.EmptyProducts()
            : ResponseExtensions.EmptyProducts();
}