using System.Diagnostics.CodeAnalysis;
using Demo.MiniProducts.Api.Core;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

[ExcludeFromCodeCoverage]
public class Validator : ModelValidatorBase<ChangeLocationRequest>
{
    public Validator() => NotNullOrEmpty(x => x.Id, x => x.LocationCode, x => x.Id);
}
