using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Features.FindById;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentValidation.Results;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api.Extensions;

public static class ResponseExtensions
{
    public static ValidationProblem ToValidationErrorResponse(
        this ValidationResult validationResult,
        string category = "InvalidRequest",
        string title = "Invalid Request"
    ) => ValidationProblem(validationResult.ToDictionary(), type: category, title: title);

    public static ProblemHttpResult ProductNotFound(string message = "product not found") =>
        Problem(
            new ProblemDetails
            {
                Type = "NotFound",
                Title = "Product not found",
                Detail = "product cannot be found",
                Status = StatusCodes.Status404NotFound,
                Extensions = { { "ProductId", message } }
            }
        );

    public static ProblemHttpResult ToErrorResponse(this QueryResult.QueryFailedResult failure) =>
        Problem(
            new ProblemDetails
            {
                Type = "Error",
                Title = failure.ErrorCode.ToString(),
                Detail = failure.ErrorMessage,
                Status = StatusCodes.Status500InternalServerError
            }
        );

    public static ProblemHttpResult ToErrorResponse(this ApiOperation.ApiFailedOperation failure) =>
        Problem(
            new ProblemDetails
            {
                Type = "Error",
                Title = failure.ErrorCode.ToString(),
                Detail = failure.ErrorMessage,
                Status = StatusCodes.Status500InternalServerError
            }
        );

    public static ProblemHttpResult ToErrorResponse(this Error error) =>
        Problem(
            new ProblemDetails
            {
                Type = "Error",
                Title = error.Code.ToString(),
                Detail = error.Message,
                Status = StatusCodes.Status500InternalServerError
            }
        );

    public static ProductRegisteredEvent ToEvent(this RegisterProductRequest request) =>
        new(request.ProductId, request.Category, DateTime.UtcNow);

    public static Features.FindById.ProductResponse ToProductResponse(this ProductDataModel dataModel) =>
        new(
            new ProductDto(
                dataModel.ProductId,
                dataModel.Name,
                dataModel.LocationCode,
                dataModel.Category
            )
        );
    
    public static ProductDataModel ToDataModel(this RegisterProductRequest request) =>
        ProductDataModel.New(
            request.Category,
            request.ProductId,
            request.Name,
            request.LocationCode
        );
}
