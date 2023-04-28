using System.Net;
using System.Net.Http.Json;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using Demo.MiniProducts.Automation.Tests.Overrides;
using FluentAssertions;
using Funky.Azure.DataTable.Extensions.Commands;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Storage.Queue.Helper;

namespace Demo.MiniProducts.Automation.Tests.RegisterProduct;

public class EndPointTests
{
    private readonly HttpClient _client;

    public EndPointTests()
    {
        TestWebApplicationFactory<Api.Program> factory =
            new(services =>
            {
                var mockedCommandService = new Mock<ICommandService>();
                mockedCommandService
                    .Setup(
                        x =>
                            x.UpsertAsync(
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<ProductDataModel>(),
                                It.IsAny<CancellationToken>()
                            )
                    )
                    .ReturnsAsync(CommandOperation.Success());

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

                services.AddSingleton(mockedCommandService.Object);
                services.AddSingleton(mockedQueueService.Object);
            });

        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "Invalid request")]
    public async Task InvalidRequest()
    {
        var request = new RegisterProductRequest
        {
            Name = string.Empty,
            Category = "tech",
            LocationCode = "1234",
            ProductId = "prod-666"
        };
        var response = await HttpMethodCaller.PostAsync(
            _client,
            "/products",
            () => JsonContent.Create(request)
        );
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Valid request must register product successfully")]
    public async Task RegistersProductSuccessfully()
    {
        var request = new RegisterProductRequest
        {
            Name = "666",
            Category = "tech",
            LocationCode = "666",
            ProductId = "666"
        };
        var response = await HttpMethodCaller.PostAsync(
            _client,
            "/products",
            () => JsonContent.Create(request)
        );
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
