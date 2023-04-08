using System.Text.Json;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storage.Queue.Helper;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api.Features.RegisterProduct;

public record RegisterProductRequest(
    [FromBody] string Name,
    [FromBody] string LocationCode,
    [FromBody] string Category
);

public static class Service
{
    private const string Route = "products";

    public static async Task<Results<ValidationProblem, Created>> RegisterProduct(
        [FromBody] RegisterProductRequest request,
        [FromServices] ProductsDbContext context,
        [FromServices] IValidator<RegisterProductRequest> validator,
        [FromServices] RegisterProductSettings settings,
        [FromServices] IQueueService queueService
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return validationResult.ToValidationErrorResponse();

        var registeredProduct = await context.Products.AddAsync(request.ToWriteModel());
        await context.SaveChangesAsync();

        var @event = new ProductRegisteredEvent(
            registeredProduct.Entity.Id.ToString(),
            request.Category,
            DateTime.UtcNow
        );
        var op = await queueService.PublishAsync(
            settings.Category,
            new CancellationToken(),
            (settings.Queue, () => JsonSerializer.Serialize(@event))
        );

        return Created($"/{Route}/{registeredProduct.Entity.Id}");
    }
}