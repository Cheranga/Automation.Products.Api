using Demo.MiniProducts.Api.Models;
using Demo.MiniProducts.Api.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api;

public static class ProductApi
{
    public const string Route = "products";

    public static async Task<
        Results<ProblemHttpResult, Ok<ApiResponse<List<Product>>>>
    > GetAllProducts(ProductsDbContext context) =>
        await context.Products.ToListAsync() is { } products
            ? products.Any()
                ? Ok(ApiResponse<List<Product>>.New(products))
                : ResponseExtensions.EmptyProducts()
            : ResponseExtensions.EmptyProducts();

    public static async Task<Results<ProblemHttpResult, Ok<ApiResponse<Product>>>> GetProduct(
        int id,
        ProductsDbContext context
    ) =>
        await context.Products.FindAsync(id) is { } product
            ? Ok(ApiResponse<Product>.New(product))
            : ResponseExtensions.ProductUnfound(id);

    public static async Task<Results<ValidationProblem, Created>> Create(
        Product product,
        ProductsDbContext context,
        IValidator<Product> validator
    )
    {
        var validationResult = await validator.ValidateAsync(product);
        if (!validationResult.IsValid)
            return validationResult.ToValidationErrorResponse();

        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        return Created($"/{Route}/{product.Id}");
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
