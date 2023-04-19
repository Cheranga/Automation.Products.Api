using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;
using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.Extensions;
using Swashbuckle.AspNetCore.Filters;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

[ExcludeFromCodeCoverage]
public record ChangeLocationRequestDto([property: Required] string LocationCode) : IDto<ChangeLocationRequestDto>,IExamplesProvider<ChangeLocationRequestDto>
{
    public ChangeLocationRequestDto():this(string.Empty)
    {
    }

    public ChangeLocationRequestDto GetExamples() => new("6666");
}

[ExcludeFromCodeCoverage]
public record ChangeLocationRequest(string Category, string Id, string LocationCode);

//     public static async ValueTask<ChangeLocationRequest> BindAsync(
//         HttpContext context,
//         ParameterInfo _
//     )
//     {
//         var category = context.GetRouteValue(nameof(Category))?.ToString();
//         var productId = context.GetRouteValue(nameof(Id))?.ToString();
//         var record = await context.Request.Body.ToModel<ChangeLocationRequest>() with
//         {
//             Category = category ?? string.Empty,
//             Id = productId ?? string.Empty
//         };
//
//         return record;
//     }
// }
