using System.Net;
using Demo.MiniProducts.Api.Features.ChangeLocation;
using Demo.MiniProducts.Automation.Tests.Overrides;
using FluentAssertions;

namespace Demo.MiniProducts.Automation.Tests.ChangeLocation;

public class EndPointTests : TestBase
{
    
    private const string Products = "/products";

    public EndPointTests(TestWebApplicationFactory<Api.Program> factory) : base(factory) { }

    [Theory(DisplayName = "Invalid request")]
    [InlineData("")]
    [InlineData(null)]
    public async Task InvalidRequest(string locationCode)
    {
        new TestWebApplicationFactory<Program>()
        var request = new ChangeLocationRequestDto { LocationCode = locationCode };
        var response = await PutAsync(
            $"{Products}/location/tech/prod1",
            request,
            Array.Empty<(string, string)>
        );
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "product does not exist")]
    public async Task ProductDoesNotExist()
    {
        var request = new ChangeLocationRequestDto { LocationCode = "666" };
        var response = await PutAsync(
            $"{Products}/location/tech/prod1",
            request,
            Array.Empty<(string, string)>
        );
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = "Product exists, and published updated event")]
    public async Task ProductUpdated()
    {
        throw new NotImplementedException();
    }
}
