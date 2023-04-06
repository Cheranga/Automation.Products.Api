using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

public record ChangeLocationRequest([FromRoute] int Id, [FromBody] string LocationCode);

public static class Service
{
    public static async Task<Results<ValidationProblem, NotFound, NoContent>> ChangeLocation(
        [FromRoute] int id,
        [FromBody] ChangeLocationRequest request,
        [FromServices] IValidator<ChangeLocationRequest> validator,
        [FromServices] ProductsDbContext context
    )
    {
        request = request with {Id = id};
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return validationResult.ToValidationErrorResponse();
    
        return await context.Products.FindAsync(id) is { } product
            ? await UpdateLocation(context, product, request)
            : NotFound();
    }

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