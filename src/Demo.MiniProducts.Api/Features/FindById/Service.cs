using Azure.Storage.Table.Wrapper.Queries;
using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;
using static Demo.MiniProducts.Api.Extensions.ApiOperationExtensions;

namespace Demo.MiniProducts.Api.Features.FindById;

public record ProductDto(string Id, string Name, string Location, string Category);

public record ProductResponse(ProductDto Data) : ResponseDtoBase<ProductDto>(Data);

public static class Service
{
    public static async Task<
        Results<ProblemHttpResult, NotFound, Ok<ProductResponse>>
    > GetProductDetailsById(
        [FromRoute] string category,
        [FromRoute] string id,
        [FromServices] RegisterProductSettings settings,
        [FromServices] IQueryService queryService
    )
    {
        var operation = await GetEntity<ProductDataModel>(
            settings.Category,
            settings.Table,
            category.ToUpper(),
            id.ToUpper(),
            queryService
        );

        return operation.Operation switch
        {
            ApiOperation.ApiFailedOperation f => f.ToErrorResponse(),
            ApiOperation.ApiSuccessfulOperation => NotFound(),
            ApiOperation.ApiSuccessfulOperation<ProductDataModel> p
                => Ok(p.Data.ToProductResponse()),
            _ => throw new NotSupportedException()
        };
    }
}
