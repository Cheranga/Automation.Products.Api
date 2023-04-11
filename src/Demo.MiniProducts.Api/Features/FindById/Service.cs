using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storage.Table.Helper;
using static Microsoft.AspNetCore.Http.TypedResults;
using ResponseExtensions = Demo.MiniProducts.Api.Extensions.ResponseExtensions;

namespace Demo.MiniProducts.Api.Features.FindById;

public record ProductDto(string Id, string Name, string Location, string Category);

public record ProductResponse(ProductDto Data) : ResponseDtoBase<ProductDto>(Data);

public static class Service
{
    public static async Task<Results<ProblemHttpResult, Ok<ProductResponse>>> GetProductDetailsById(
        [FromRoute] string category,
        [FromRoute] string id,
        [FromServices] RegisterProductSettings settings,
        [FromServices] ITableService tableService
    )
    {
        var op = await tableService.GetEntityAsync<ProductDataModel>(
            settings.Category,
            settings.Table,
            category.ToUpper(),
            id.ToUpper(),
            new CancellationToken()
        );
        if (op is TableOperation.FailedOperation)
        {
            return ResponseExtensions.ProductUnfound(id);
        }

        return op switch
        {
            TableOperation.QuerySingleOperation<ProductDataModel> q
                => Ok(
                    new ProductResponse(
                        new ProductDto(
                            q.Entity.ProductId,
                            q.Entity.Name,
                            q.Entity.LocationCode,
                            q.Entity.Category
                        )
                    )
                ),
            TableOperation.FailedOperation f
                => Problem(
                    new ProblemDetails
                    {
                        Type = "Error",
                        Title = "Error",
                        Detail = "error occurred when getting product",
                        Status = StatusCodes.Status500InternalServerError,
                        Extensions =
                        {
                            { "ErrorCode", f.Error.Code },
                            { "ErrorMessage", f.Error.Message }
                        }
                    }
                ),
            _ => throw new NotSupportedException("")
        };
    }
}
