using System.Text.Json;
using Azure.Storage.Table.Wrapper.Commands;
using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storage.Queue.Helper;
using static Microsoft.AspNetCore.Http.TypedResults;
using static Demo.MiniProducts.Api.Extensions.ApiOperationExtensions;

namespace Demo.MiniProducts.Api.Features.RegisterProduct;

public static class Service
{
    private const string Route = "products";

    public static async Task<
        Results<ValidationProblem, ProblemHttpResult, Created>
    > RegisterProduct(
        [FromBody] RegisterProductRequest request,
        [FromServices] IValidator<RegisterProductRequest> validator,
        [FromServices] RegisterProductSettings settings,
        [FromServices] IQueueService queueService,
        [FromServices] ICommandService commandService,
        CancellationToken token = new()
    )
    {
        var validation = await Validate(request, validator, token);
        if (validation.Operation is ApiOperation.ApiValidationFailureOperation vr)
        {
            return vr.ValidationResult.ToValidationErrorResponse();
        }

        var upsert = await Upsert(
            settings.Category,
            settings.Table,
            request.ToDataModel(),
            commandService,
            token
        );
        if (upsert.Operation is ApiOperation.ApiFailedOperation f)
        {
            return f.ToErrorResponse();
        }

        var publishEvent = await PublishEvent(
            () => JsonSerializer.Serialize(request.ToEvent()),
            settings.Category,
            settings.Queue,
            queueService,
            token
        );
        if (publishEvent.Operation is ApiOperation.ApiFailedOperation fp)
        {
            return fp.ToErrorResponse();
        }

        return Created($"/{Route}/{request.Category}/{request.ProductId}");
    }
}
