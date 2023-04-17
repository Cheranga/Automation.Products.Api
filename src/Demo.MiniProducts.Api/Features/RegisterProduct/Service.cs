using System.Text.Json;
using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.Extensions;
using FluentValidation;
using FluentValidation.Results;
using Funky.Azure.DataTable.Extensions.Commands;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storage.Queue.Helper;
using static LanguageExt.Prelude;
using static Microsoft.AspNetCore.Http.TypedResults;
using CR = Funky.Azure.DataTable.Extensions.Commands.CommandResponse<
    Funky.Azure.DataTable.Extensions.Commands.CommandOperation.CommandFailedOperation,
    Funky.Azure.DataTable.Extensions.Commands.CommandOperation.CommandSuccessOperation
>;

namespace Demo.MiniProducts.Api.Features.RegisterProduct;

public static class Service
{
    private const string Route = "products";

    public static async Task<
        Results<ValidationProblem, ProblemHttpResult, Created>
    > RegisterProduct(
        RegisterProductRequest request,
        IValidator<RegisterProductRequest> validator,
        RegisterProductSettings settings,
        IQueueService queueService,
        ICommandService commandService,
        ILogger logger,
        CancellationToken token = new()
    ) =>
        (
            await (
                from valOp in ValidateRequest(request, validator, token)
                from upsertOp in UpsertDataToTable(
                    settings.Category,
                    settings.Table,
                    request,
                    commandService,
                    token
                )
                from publishEventOp in PublishEventToQueue(
                    () => JsonSerializer.Serialize(request.ToDataModel()),
                    settings.Category,
                    settings.Queue,
                    queueService,
                    token
                )
                select publishEventOp
            ).Run()
        ).Match(
            _ =>
            {
                logger.LogInformation(
                    "Product registration accepted @{Category} @{ProductId}",
                    request.Category,
                    request.ProductId
                );

                return Created($"/{Route}/{request.Category}/{request.ProductId}");
            },
            err =>
            {
                logger.LogError("Error occurred {@Error}", err);
                return GetErrorResponse(err);
            }
        );

    private static Results<ValidationProblem, ProblemHttpResult, Created> GetErrorResponse(
        Error error
    ) =>
        error switch
        {
            ApiError<ValidationResult> ve => ve.Data.ToValidationErrorResponse(),
            ApiError<CommandOperation.CommandFailedOperation> ce
                => Problem(
                    new ProblemDetails
                    {
                        Type = "Error",
                        Title = ce.Data.ErrorCode.ToString(),
                        Detail = ce.Data.ErrorMessage,
                        Status = StatusCodes.Status500InternalServerError
                    }
                ),
            ApiError<QueueOperation.FailedOperation> ce
                => Problem(
                    new ProblemDetails
                    {
                        Type = "Error",
                        Title = ce.Data.Error.Code.ToString(),
                        Detail = ce.Data.Error.Message,
                        Status = StatusCodes.Status500InternalServerError
                    }
                ),
            _ => error.ToErrorResponse()
        };

    private static Aff<ValidationResult> ValidateRequest(
        RegisterProductRequest request,
        IValidator<RegisterProductRequest> validator,
        CancellationToken token
    ) =>
        from vr in AffMaybe<ValidationResult>(
            async () => await validator.ValidateAsync(request, token)
        )
        from _ in guard(vr.IsValid, ApiError<ValidationResult>.New(vr))
        select vr;

    private static Aff<CR> UpsertDataToTable(
        string category,
        string table,
        RegisterProductRequest request,
        ICommandService commandService,
        CancellationToken token
    ) =>
        from op in AffMaybe<CR>(
            async () =>
                await commandService.UpsertAsync(category, table, request.ToDataModel(), token)
        )
        from _ in guardnot(
            op.Operation is CommandOperation.CommandFailedOperation,
            ApiError<CommandOperation.CommandFailedOperation>.New(
                (op.Operation as CommandOperation.CommandFailedOperation)!
            )
        )
        select op;

    private static Aff<QueueOperation> PublishEventToQueue(
        Func<string> eventContent,
        string category,
        string queue,
        IQueueService queueService,
        CancellationToken token
    ) =>
        from op in AffMaybe<QueueOperation>(
            async () => await queueService.PublishAsync(category, token, (queue, eventContent))
        )
        from _ in guardnot(
            op is QueueOperation.FailedOperation,
            ApiError<QueueOperation.FailedOperation>.New((op as QueueOperation.FailedOperation)!)
        )
        select op;
}
