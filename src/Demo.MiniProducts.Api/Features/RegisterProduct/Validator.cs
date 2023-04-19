using System.Diagnostics.CodeAnalysis;
using Demo.MiniProducts.Api.Core;

namespace Demo.MiniProducts.Api.Features.RegisterProduct;

[ExcludeFromCodeCoverage]
public class Validator : ModelValidatorBase<RegisterProductRequest>
{
    public Validator() =>
        NotNullOrEmpty(x => x.ProductId, x => x.LocationCode, x => x.Category, x => x.Name);
}
