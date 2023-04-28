using System.Net;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Features.FindById;
using Demo.MiniProducts.Automation.Tests.Overrides;
using FluentAssertions;
using Funky.Azure.DataTable.Extensions.Queries;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Demo.MiniProducts.Automation.Tests.FindProductById;

public class EndPointTests
{
    private const string Products = "/products";
    private readonly HttpClient _client;

    public EndPointTests()
    {
        TestWebApplicationFactory<Program> factory =
            new(services =>
            {
                services.AddSingleton<IQueryService>(
                    new TestQueryService(
                        new List<object> { ProductDataModel.New("tech", "prod1", "item1", "1") }
                    )
                );
            });

        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "Product exists")]
    public async Task ProductExists()
    {
        var response = await _client.GetAsync($"{Products}/tech/prod1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var productResponse = JsonConvert.DeserializeObject<ProductResponse>(
            await response.Content.ReadAsStringAsync()
        );
        productResponse.Should().NotBeNull();
        productResponse.Data.Should().NotBeNull();
        productResponse.Data.Id.Should().Be("prod1");
    }

    [Fact(DisplayName = "Product does not exist")]
    public async Task ProductDoesNotExist()
    {
        var response = await _client.GetAsync($"{Products}/tech/prod-666");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
