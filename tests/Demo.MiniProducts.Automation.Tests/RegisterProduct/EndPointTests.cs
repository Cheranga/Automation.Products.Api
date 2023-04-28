// using System.Net;
// using Demo.MiniProducts.Api.DataAccess;
// using Demo.MiniProducts.Api.Features.RegisterProduct;
// using Demo.MiniProducts.Automation.Tests.Overrides;
// using FluentAssertions;
// using Funky.Azure.DataTable.Extensions.Commands;
// using Microsoft.Extensions.DependencyInjection;
// using Moq;
//
// namespace Demo.MiniProducts.Automation.Tests.RegisterProduct;
//
// public class EndPointTests : TestBase
// {
//     public EndPointTests(TestWebApplicationFactory<Api.Program> factory) : base(factory) { }
//
//     [Fact(DisplayName = "Invalid request")]
//     public async Task InvalidRequest()
//     {
//         var request = new RegisterProductRequest
//         {
//             Name = string.Empty,
//             Category = "tech",
//             LocationCode = "1234",
//             ProductId = "prod-666"
//         };
//         var response = await PostAsync("/products", request, Array.Empty<(string, string)>);
//         response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//     }
//
//     [Fact(DisplayName = "Valid request must register product successfully")]
//     public async Task RegistersProductSuccessfully()
//     {
//         var request = new RegisterProductRequest
//         {
//             Name = "666",
//             Category = "tech",
//             LocationCode = "666",
//             ProductId = "666"
//         };
//         var response = await PostAsync("/products", request, Array.Empty<(string, string)>);
//         response.StatusCode.Should().Be(HttpStatusCode.Created);
//     }
// }
