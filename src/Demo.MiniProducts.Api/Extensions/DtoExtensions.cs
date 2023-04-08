using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Features.FindById;
using Demo.MiniProducts.Api.Features.RegisterProduct;

namespace Demo.MiniProducts.Api.Extensions;

public static class DtoExtensions
{
    public static ProductDto ToDto(this Product product) =>
        new(
            product.Id.ToString(),
            product.Name,
            product.LocationCode,
            product.Category,
            product.IsActive,
            product.IsOnPromotion
        );

    public static List<ProductDto> ToDto(this IEnumerable<Product> products) =>
        products.Select(x => x.ToDto()).ToList();

    public static Product ToWriteModel(this RegisterProductRequest dto) =>
        new()
        {
            Name = dto.Name,
            Category = dto.Category,
            LocationCode = dto.LocationCode,
            IsActive = true,
            IsOnPromotion = false
        };
}
