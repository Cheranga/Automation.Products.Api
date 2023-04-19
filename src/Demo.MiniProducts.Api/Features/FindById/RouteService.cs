using Demo.MiniProducts.Api.Extensions;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using static Demo.MiniProducts.Api.Features.FindById.Service;
namespace Demo.MiniProducts.Api.Features.FindById;

public class RouteService
{
    public static void Setup(RouteGroupBuilder builder)=>
    builder.MapGet(
                   "/{category}/{id}",
                   (
                       [FromRoute] string category,
                       [FromRoute] string id,
                       [FromServices] RegisterProductSettings settings,
                       [FromServices] IQueryService queryService,
                       [FromServices] ILoggerFactory loggerFactory
                   ) =>
                       GetProductDetailsById(
                           new GetProductByIdRequest(category, id),
                           settings,
                           queryService,
                           loggerFactory.CreateLogger("FindProductsById")
                       )
               )
               .WithName(nameof(GetProductDetailsById))
               .WithSummary("Get product by product id.")
               .WithOpenApi(operation =>
               {
                   operation.OperationId = nameof(GetProductDetailsById);
                   return operation
                       .SetParamInfo<GetProductByIdRequest>(
                           x => x.Category,
                           x =>
                           {
                               x.In = ParameterLocation.Path;
                               x.Required = true;
                               x.Description = "The category of the product.";
                               x.Example = new OpenApiString("TECH");
                           }
                       )
                       .SetParamInfo<GetProductByIdRequest>(
                           x => x.Id,
                           x =>
                           {
                               x.In = ParameterLocation.Path;
                               x.Required = true;
                               x.Description = "The id of the product.";
                               x.Example = new OpenApiString("PROD1");
                           }
                       );
               });
}