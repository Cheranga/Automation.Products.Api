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
        await context.Products.FindAsync(id) is { } todo
            ? Ok(todo)
            : NotFound();

    public static async Task<IResult> Create(Product todo, ProductsDbContext context)
    {
        await context.Products.AddAsync(todo);
        await context.SaveChangesAsync();

        return Created($"/todos/{todo.Id}", todo);
    }

    public static async Task<IResult> Update(int id, Product updated, ProductsDbContext context) =>
        await context.Products.FindAsync(id) is { } todo
            ? await UpdateProduct(context, todo, updated)
            : NotFound();

    private static async Task<IResult> UpdateProduct(ProductsDbContext context, Product todo, Product updated)
    {
        todo.Name = updated.Name;
        todo.IsComplete = updated.IsComplete;
        await context.SaveChangesAsync();
        return NoContent();
    }
}
