using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Demo.MiniProducts.Api.Core;
using Swashbuckle.AspNetCore.Filters;

namespace Demo.MiniProducts.Api.Features.FindById;

[ExcludeFromCodeCoverage]
public class GetProductByIdRequest
    : IDto<GetProductByIdRequest>,
        IExamplesProvider<GetProductByIdRequest>
{
    public GetProductByIdRequest(string category, string id)
    {
        Category = category;
        Id = id;
    }

    public GetProductByIdRequest() : this(string.Empty, string.Empty) { }

    [Required]
    public string Category { get; set; }

    [Required]
    public string Id { get; set; }

    public GetProductByIdRequest GetExamples() => new("TECH", "PROD1");

    public static ValueTask<GetProductByIdRequest> BindAsync(HttpContext context, ParameterInfo _)
    {
        var category = context.GetRouteValue(nameof(Category))?.ToString() ?? string.Empty;
        var productId = context.GetRouteValue(nameof(Id))?.ToString() ?? string.Empty;

        return ValueTask.FromResult(new GetProductByIdRequest(category, productId));
    }
}
