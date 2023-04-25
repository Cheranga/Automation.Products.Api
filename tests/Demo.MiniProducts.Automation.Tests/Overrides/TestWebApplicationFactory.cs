using Demo.MiniProducts.Api.DataAccess;
using Funky.Azure.DataTable.Extensions.Commands;
using Funky.Azure.DataTable.Extensions.Queries;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Storage.Queue.Helper;

namespace Demo.MiniProducts.Automation.Tests.Overrides;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var mockedQueueService = new Mock<IQueueService>();
            mockedQueueService
                .Setup(
                    x =>
                        x.PublishAsync(
                            It.IsAny<string>(),
                            It.IsAny<CancellationToken>(),
                            It.IsAny<(string queue, Func<string> content)>()
                        )
                )
                .ReturnsAsync(QueueOperation.Success);
            services.AddSingleton(mockedQueueService.Object);
            services.AddSingleton<ICommandService>(new TestCommandService());
            services.AddSingleton<IQueryService>(
                new TestQueryService(
                    new List<object>
                    {
                        ProductDataModel.New("tech", "prod1", "item1", "1"),
                        ProductDataModel.New("tech", "prod2", "item2", "2"),
                        ProductDataModel.New("tech", "prod3", "item3", "3"),
                        ProductDataModel.New("tech", "prod4", "item4", "4"),
                        ProductDataModel.New("tech", "prod5", "item5", "5")
                    }
                )
            );
        });
    }
}
