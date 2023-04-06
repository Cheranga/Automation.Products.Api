using Demo.MiniProducts.Api.Dto;
using Demo.MiniProducts.Api.Models;
using Demo.MiniProducts.Api.Requests;

namespace Demo.MiniProducts.Api.Extensions;

public static class DtoExtensions
{
    public static ProductDto ToDto(this Product product) =>
        new(
            product.Id.ToString(),
            product.Name,
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
            IsActive = true,
            IsOnPromotion = false
        };
}
