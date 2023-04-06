using Demo.MiniProducts.Api.Dto;

namespace Demo.MiniProducts.Api.Responses;

public abstract record ResponseDtoBase<T>(T Data) where T : notnull;

public record ProductResponse(ProductDto Data) : ResponseDtoBase<ProductDto>(Data);

public record ProductListResponse(List<ProductDto> Data) : ResponseDtoBase<List<ProductDto>>(Data);
