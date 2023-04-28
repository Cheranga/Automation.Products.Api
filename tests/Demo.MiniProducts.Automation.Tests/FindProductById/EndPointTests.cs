// using System.Net;
// using Demo.MiniProducts.Api.DataAccess;
// using Demo.MiniProducts.Api.Features.FindById;
// using Demo.MiniProducts.Automation.Tests.Overrides;
// using FluentAssertions;
// using Funky.Azure.DataTable.Extensions.Queries;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.Extensions.DependencyInjection;
// using Newtonsoft.Json;
//
// namespace Demo.MiniProducts.Automation.Tests.FindProductById;
//
// public class EndPointTests //: TestBase
// {
//     private const string Products = "/products";
//     private readonly TestWebApplicationFactory<Program> _factory;
//
//     public EndPointTests() =>
//         _factory = new TestWebApplicationFactory<Program>(services =>
//         {
//             services.AddSingleton<IQueryService>(
//                 new TestQueryService(
//                     new List<object>
//                     {
//                         ProductDataModel.New("tech", "prod1", "item1", "1"),
//                         ProductDataModel.New("tech", "prod2", "item2", "2"),
//                         ProductDataModel.New("tech", "prod3", "item3", "3"),
//                         ProductDataModel.New("tech", "prod4", "item4", "4"),
//                         ProductDataModel.New("tech", "prod5", "item5", "5")
//                     }
//                 ));
//         });
//
//     // public EndPointTests(TestWebApplicationFactory<Api.Program> startUp) : base(startUp) { }
//
//     [Fact(DisplayName = "Product exists")]
//     public async Task ProductExists()
//     {
//         var response = await GetAsync<ProductResponse>(
//             $"{Products}/tech/prod1",
//             Array.Empty<(string, string)>
//         );
//
//         response.StatusCode.Should().Be(HttpStatusCode.OK);
//         var productResponse = JsonConvert.DeserializeObject<ProductResponse>(
//             await response.Content.ReadAsStringAsync()
//         );
//         productResponse.Should().NotBeNull();
//         productResponse.Data.Should().NotBeNull();
//         productResponse.Data.Id.Should().Be("prod1");
//     }
//
//     [Fact(DisplayName = "Product does not exist")]
//     public async Task ProductDoesNotExist()
//     {
//         var response = await GetAsync<ProductResponse>(
//             $"{Products}/tech/blah",
//             Array.Empty<(string, string)>
//         );
//
//         response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//     }
// }