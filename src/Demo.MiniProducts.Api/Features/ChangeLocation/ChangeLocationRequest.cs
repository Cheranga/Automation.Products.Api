using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Demo.MiniProducts.Api.Core;
using Swashbuckle.AspNetCore.Filters;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

[ExcludeFromCodeCoverage]
public record ChangeLocationRequestDto(string LocationCode) : IDtoRequest<ChangeLocationRequestDto>,IExamplesProvider<ChangeLocationRequestDto>,
    IValidatable<ChangeLocationRequestDto, ChangeLocationRequestDtoValidator>
{
    [Required]
    public string LocationCode { get; set; } = LocationCode;

    public ChangeLocationRequestDto():this(string.Empty)
    {
    }

    public ChangeLocationRequestDto GetExamples() => new("6666");
}

[ExcludeFromCodeCoverage]
public record ChangeLocationRequest(string Category, string Id, string LocationCode);