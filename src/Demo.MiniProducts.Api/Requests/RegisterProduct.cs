using Microsoft.AspNetCore.Mvc;

namespace Demo.MiniProducts.Api.Requests;

public record RegisterProductRequest(
    [FromBody] string Name,
    [FromBody] string LocationCode,
    [FromBody] string Category
);

public record ChangeLocationRequest([FromRoute] string Id, [FromBody] string LocationCode);

public record ChangeCategoryRequest(string Id, string Category);

public record GetProductDetailsRequest([FromRoute] string Id);
