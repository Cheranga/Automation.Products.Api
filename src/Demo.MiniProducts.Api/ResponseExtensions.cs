using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Demo.MiniProducts.Api;

public static class ResponseExtensions
{
    public static ValidationProblem ToErrorResponse(
        this ValidationResult validationResult,
        string category = "InvalidRequest",
        string title = "Invalid Request"
    ) => ValidationProblem(validationResult.ToDictionary(), type: category, title: title);
}
