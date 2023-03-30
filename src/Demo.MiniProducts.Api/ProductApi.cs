using Demo.MiniProducts.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;
using static Demo.MiniProducts.Api.Models.ErrorResponse;

namespace Demo.MiniProducts.Api;

public static class ProductApi
{
    public const string Route = "products";

    public static async Task<Results<NotFound<NotFoundResponse>, Ok<List<Product>>>> GetAllProducts(
        ProductsDbContext context
    ) =>
        await context.Products.ToListAsync() is { } products
            ? products.Any()
                ? Ok(products)
                : NotFound(ErrorResponse.NotFound())
            : NotFound(ErrorResponse.NotFound());

    public static async Task<Results<NotFound<NotFoundResponse>, Ok<Product>>> GetProduct(
        int id,
        ProductsDbContext context
    ) =>
        await context.Products.FindAsync(id) is { } product
            ? Ok(product)
            : NotFound(ErrorResponse.NotFound());

    public static async Task<IResult> Create(Product product, ProductsDbContext context)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        return Created($"/{Route}/{product.Id}", product);
    }

    public static async Task<IResult> Update(int id, Product updated, ProductsDbContext context) =>
        await context.Products.FindAsync(id) is { } product
            ? await UpdateProduct(context, product, updated)
            : NotFound();

    private static async Task<IResult> UpdateProduct(
        ProductsDbContext context,
        Product product,
        Product updated
    )
    {
        product.Name = updated.Name;
        product.IsComplete = updated.IsComplete;
        await context.SaveChangesAsync();
        return NoContent();
    }
}
