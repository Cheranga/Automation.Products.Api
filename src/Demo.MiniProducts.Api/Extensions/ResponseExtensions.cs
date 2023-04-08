using System.Net;
using FluentValidation.Results;
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

    public static ProblemHttpResult EmptyProducts() =>
        Problem(
            new ProblemDetails
            {
                Type = "NotFound",
                Title = "Products not found",
                Detail = "products cannot be found",
                Status = (int) HttpStatusCode.NotFound
            }
        );

    public static ProblemHttpResult ProductUnfound(
        string productId,
        string message = "product not found"
    ) =>
        Problem(
            new ProblemDetails
            {
                Type = "NotFound",
                Title = "Product not found",
                Detail = "product cannot be found",
                Status = (int) HttpStatusCode.NotFound,
                Extensions = {{"ProductId", message}}
            }
        );
}