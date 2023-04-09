using System.Reflection;
using Demo.MiniProducts.Api.Extensions;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

public record ChangeLocationRequest
{
    public string Category { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;

    public static async ValueTask<ChangeLocationRequest> BindAsync(
        HttpContext context,
        ParameterInfo _
    )
    {
        var category = context.GetRouteValue(nameof(Category))?.ToString();
        var productId = context.GetRouteValue(nameof(Id))?.ToString();
        var record = await context.Request.Body.ToModel<ChangeLocationRequest>() with
        {
            Category = category?? string.Empty,
            Id = productId?? string.Empty
        };
        
        return record;
    }
}