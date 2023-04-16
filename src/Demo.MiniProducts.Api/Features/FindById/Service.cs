using Azure.Storage.Table.Wrapper.Queries;
using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using static LanguageExt.Prelude;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api.Features.FindById;

public record ProductDto(string Id, string Name, string Location, string Category);

public record ProductResponse(ProductDto Data) : ResponseDtoBase<ProductDto>(Data);

public static class Service
{
    public static async Task<
        Results<ProblemHttpResult, NotFound, Ok<ProductResponse>>
    > GetProductDetailsById(
        string category,
        string id,
        RegisterProductSettings settings,
        IQueryService queryService,
        ILogger logger,
        CancellationToken token = new()
    ) =>
        (
            await (
                from op in Get(
                    queryService,
                    settings.Category,
                    settings.Table,
                    category.ToUpper(),
                    id.ToUpper(),
                    token
                )
                select op
            ).Run()
        ).Match(
            op =>
            {
                logger.LogInformation("@{Category} @{ProductId} found", category, id);
                return ToApiResponse(op.Response);
            },
            err =>
            {
                logger.LogError("@{Error} occurred", err);
                return Error
                    .New(ErrorCodes.FindProductError, ErrorMessages.FindProductError)
                    .ToErrorResponse();
            }
        );

    private static Aff<GetProductResponse> Get(
        IQueryService queryService,
        string category,
        string table,
        string partitionKey,
        string rowKey,
        CancellationToken token
    )
    {
        return AffMaybe<GetProductResponse>(
            async () =>
                await queryService.GetEntityAsync<ProductDataModel>(
                    category,
                    table,
                    partitionKey.ToUpper(),
                    rowKey.ToUpper(),
                    token
                )
        );
    }

    private static Results<ProblemHttpResult, NotFound, Ok<ProductResponse>> ToApiResponse(
        QueryResult result
    ) =>
        result switch
        {
            QueryResult.QueryFailedResult f => f.ToErrorResponse(),
            QueryResult.EmptyResult => NotFound(),
            QueryResult.SingleResult<ProductDataModel> p => Ok(p.Entity.ToProductResponse()),
            _ => throw new NotSupportedException()
        };
}
