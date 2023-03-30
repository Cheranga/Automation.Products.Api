using Demo.MiniProducts.Api.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api;

public static class ProductApi
{
    public const string Route = "products";

    public static async Task<IResult> GetAllProducts(ProductsDbContext context) =>
        Ok(await context.Products.ToListAsync());

    public static async Task<IResult> GetProduct(int id, ProductsDbContext context) =>
        await context.Products.FindAsync(id) is { } product ? Ok(product) : NotFound();

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
