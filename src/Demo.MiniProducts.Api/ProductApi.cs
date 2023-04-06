using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Models;
using Demo.MiniProducts.Api.Requests;
using Demo.MiniProducts.Api.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api;

public static class ProductApi
{
    public const string Route = "products";

    public static async Task<Results<ProblemHttpResult, Ok<ProductListResponse>>> GetAllProducts(
        [FromServices] ProductsDbContext context
    ) =>
        await context.Products.ToListAsync() is { } products
            ? products.Any()
                ? Ok(new ProductListResponse(products.ToDto()))
                : ResponseExtensions.EmptyProducts()
            : ResponseExtensions.EmptyProducts();

    public static async Task<Results<ProblemHttpResult, Ok<ProductResponse>>> GetProductDetailsById(
        [FromRoute] int Id,
        [FromServices] ProductsDbContext context
    ) =>
        await context.Products.FindAsync(Id) is { } product
            ? Ok(new ProductResponse(product.ToDto()))
            : ResponseExtensions.ProductUnfound(Id.ToString());

    public static async Task<Results<ValidationProblem, Created>> RegisterProduct(
        [FromBody] RegisterProductRequest request,
        [FromServices] ProductsDbContext context,
        [FromServices] IValidator<RegisterProductRequest> validator
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return validationResult.ToValidationErrorResponse();

        var registeredProduct = await context.Products.AddAsync(request.ToWriteModel());
        await context.SaveChangesAsync();

        return Created($"/{Route}/{registeredProduct.Entity.Id}");
    }

    // public static async Task<Results<ValidationProblem, NotFound, NoContent>> ChangeLocation(
    //     [FromRoute] int id,
    //     [FromBody] ChangeLocationRequest request,
    //     [FromServices] IValidator<ChangeLocationRequest> validator,
    //     [FromServices] ProductsDbContext context
    // )
    // {
    //     var validationResult = await validator.ValidateAsync(request);
    //     if (!validationResult.IsValid)
    //         return validationResult.ToValidationErrorResponse();
    //
    //     return await context.Products.FindAsync(id) is { } product
    //         ? await UpdateLocation(context, product, request)
    //         : NotFound();
    // }

    private static async Task<Results<ValidationProblem, NotFound, NoContent>> UpdateLocation(
        ProductsDbContext context,
        Product product,
        ChangeLocationRequest request
    )
    {
        product.LocationCode = request.LocationCode;
        await context.SaveChangesAsync();
        return NoContent();
    }
}
