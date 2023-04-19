using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.Extensions;
using Swashbuckle.AspNetCore.Filters;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

[ExcludeFromCodeCoverage]
public record ChangeLocationRequest
    : IDto<ChangeLocationRequest>,
        IExamplesProvider<ChangeLocationRequest>
{
    public ChangeLocationRequest(string category, string id, string locationCode)
    {
        Category = category;
        Id = id;
        LocationCode = locationCode;
    }

    public ChangeLocationRequest() : this(string.Empty, string.Empty, string.Empty) { }

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string LocationCode { get; set; } = string.Empty;

    public ChangeLocationRequest GetExamples() => new("TECH", "PROD1", "6666");

    public static async ValueTask<ChangeLocationRequest> BindAsync(
        HttpContext context,
        ParameterInfo _
    )
    {
        var category = context.GetRouteValue(nameof(Category))?.ToString();
        var productId = context.GetRouteValue(nameof(Id))?.ToString();
        var record = await context.Request.Body.ToModel<ChangeLocationRequest>() with
        {
            Category = category ?? string.Empty,
            Id = productId ?? string.Empty
        };

        return record;
    }
}
