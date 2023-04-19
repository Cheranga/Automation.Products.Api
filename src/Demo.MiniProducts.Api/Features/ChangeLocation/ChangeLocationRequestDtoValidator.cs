using System.Diagnostics.CodeAnalysis;
using Demo.MiniProducts.Api.Core;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

[ExcludeFromCodeCoverage]
public class ChangeLocationRequestDtoValidator : ModelValidatorBase<ChangeLocationRequestDto>
{
    public ChangeLocationRequestDtoValidator() => NotNullOrEmpty(x => x.LocationCode);
}