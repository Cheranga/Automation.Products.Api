using Demo.MiniProducts.Api.Core;

namespace Demo.MiniProducts.Api.Features.RegisterProduct;

public class Validator : ModelValidatorBase<RegisterProductRequest>
{
    public Validator() =>
        NotNullOrEmpty(x => x.ProductId, x => x.LocationCode, x => x.Category, x => x.Name);
}
