using System.Net;
using System.Net.Http.Json;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Features.ChangeLocation;
using Demo.MiniProducts.Automation.Tests.Overrides;
using FluentAssertions;
using Funky.Azure.DataTable.Extensions.Commands;
using Funky.Azure.DataTable.Extensions.Queries;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Storage.Queue.Helper;

namespace Demo.MiniProducts.Automation.Tests.ChangeLocation;

public class EndPointTests
{
    private const string Products = "/products";
    private readonly HttpClient _client;

    public EndPointTests()
    {
        TestWebApplicationFactory<Api.Program> factory =
            new(services =>
            {
                services.AddSingleton<IQueryService>(
                    new TestQueryService(
                        new List<object> { ProductDataModel.New("tech", "prod1", "item1", "1") }
                    )
                );

                var mockedCommandService = new Mock<ICommandService>();
                mockedCommandService
                    .Setup(
                        x =>
                            x.UpdateAsync(
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<ProductDataModel>(),
                                It.IsAny<CancellationToken>()
                            )
                    )
                    .ReturnsAsync(CommandOperation.Success());
                services.AddSingleton(mockedCommandService.Object);

                var mockedQueueService = new Mock<IQueueService>();
                mockedQueueService
                    .Setup(
                        x =>
                            x.PublishAsync(
                                It.IsAny<string>(),
                                It.IsAny<CancellationToken>(),
                                It.IsAny<(string, Func<string>)>()
                            )
                    )
                    .ReturnsAsync(QueueOperation.Success);
                services.AddSingleton(mockedQueueService.Object);
            });

        _client = factory.CreateClient();
    }

    [Theory(DisplayName = "Invalid request")]
    [InlineData("")]
    [InlineData(null)]
    public async Task InvalidRequest(string locationCode)
    {
        var request = new ChangeLocationRequestDto { LocationCode = locationCode };
        var response = await HttpMethodCaller.PutAsync(
            _client,
            $"{Products}/location/tech/prod1",
            () => JsonContent.Create(request)
        );
        
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Valid request but product does not exist")]
    public async Task ProductDoesNotExist()
    {
        var request = new ChangeLocationRequestDto { LocationCode = "1234" };
        var response = await HttpMethodCaller.PutAsync(
            _client,
            $"{Products}/location/tech/prod-666",
            () => JsonContent.Create(request)
        );
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "Valid request, product exists, and published updated event")]
    public async Task ProductUpdated()
    {
        var request = new ChangeLocationRequestDto { LocationCode = "1234" };
        var response = await HttpMethodCaller.PutAsync(
            _client,
            $"{Products}/location/tech/prod1",
            () => JsonContent.Create(request)
        );
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
