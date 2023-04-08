using System.Reflection;
using System.Text.Json;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storage.Queue.Helper;
using static System.Int32;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

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
        [FromServices] ProductsDbContext context,
        [FromServices] UpdateProductSettings settings,
        [FromServices] IQueueService queueService
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return validationResult.ToValidationErrorResponse();

        return await context.Products.FindAsync(request.Id) is { } product
            ? await UpdateLocation(
                context,
                product,
                request,
                settings,
                queueService,
                new CancellationToken()
            )
            : NotFound();
    }

    private static async Task<Results<ValidationProblem, NotFound, NoContent>> UpdateLocation(
        ProductsDbContext context,
        Product product,
        ChangeLocationRequest request,
        UpdateProductSettings settings,
        IQueueService queueService,
        CancellationToken token
    )
    {
        var @event = new LocationChangedEvent(
            request.Id.ToString(),
            product.LocationCode,
            request.LocationCode
        );
        
        product.LocationCode = request.LocationCode;
        await context.SaveChangesAsync(token);

        await queueService.PublishAsync(
            settings.Category,
            token,
            (settings.Queue, () => JsonSerializer.Serialize(@event))
        );

        return NoContent();
    }
}
