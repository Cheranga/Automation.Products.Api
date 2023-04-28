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

    public static ProductResponse ToProductResponse(this ProductDataModel dataModel) =>
        new()
        {
            Data = new ProductDto
            {
                Category = dataModel.Category,
                Name = dataModel.Name,
                Id = dataModel.ProductId,
                Location = dataModel.LocationCode
            }
        };

    public static ProductDataModel ToDataModel(this RegisterProductRequest request) =>
        ProductDataModel.New(
            request.Category,
            request.ProductId,
            request.Name,
            request.LocationCode
        );
}
