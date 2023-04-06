using System.Reflection;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static System.Int32;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

// public class ChangeLocationRequest : IHybridBoundModel
// {
//     [HybridBindProperty(Source.Route)]
//     public int Id { get; set; }
//
//     [HybridBindProperty(Source.Body)]
//     public string LocationCode { get; set; } = string.Empty;
//
//     public IDictionary<string, string> HybridBoundProperties { get; } = new Dictionary<string, string>();
// }


public class ChangeLocationRequest
{
    public int Id { get; set; }
    public string LocationCode { get; set; } = string.Empty;

    public static async ValueTask<ChangeLocationRequest> BindAsync(
        HttpContext context,
        ParameterInfo _
    )
    {
        TryParse(context.GetRouteValue(nameof(Id))?.ToString(), out var id);
        var record = await context.Request.Body.ToModel<ChangeLocationRequest>();
        record.Id = id;
        return record;
    }
}

public static class Service
{
    public static async Task<Results<ValidationProblem, NotFound, NoContent>> ChangeLocation(
        ChangeLocationRequest request,
        [FromServices] IValidator<ChangeLocationRequest> validator,
        [FromServices] ProductsDbContext context
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return validationResult.ToValidationErrorResponse();

        return await context.Products.FindAsync(request.Id) is { } product
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
