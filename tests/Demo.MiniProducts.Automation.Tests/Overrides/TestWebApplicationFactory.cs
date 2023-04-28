using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.MiniProducts.Automation.Tests.Overrides;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    private readonly Action<IConfigurationBuilder>? _configurations;
    private readonly Action<IServiceCollection>? _registrations;

    public TestWebApplicationFactory(
        Action<IServiceCollection>? registrations = null,
        Action<IConfigurationBuilder>? configurations = null
    )
    {
        _registrations = registrations;
        _configurations = configurations;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (_configurations != null)
            builder.ConfigureAppConfiguration(
                configurationBuilder => _configurations(configurationBuilder)
            );

        if (_registrations != null)
            builder.ConfigureServices(services => _registrations(services));

        // builder.ConfigureServices(services =>
        // {
        //     var mockedQueueService = new Mock<IQueueService>();
        //     mockedQueueService
        //         .Setup(
        //             x =>
        //                 x.PublishAsync(
        //                     It.IsAny<string>(),
        //                     It.IsAny<CancellationToken>(),
        //                     It.IsAny<(string queue, Func<string> content)>()
        //                 )
        //         )
        //         .ReturnsAsync(QueueOperation.Success);
        //     services.AddSingleton(mockedQueueService.Object);
        //     services.AddSingleton<ICommandService>(new TestCommandService());
        //     services.AddSingleton<IQueryService>(
        //         new TestQueryService(
        //             new List<object>
        //             {
        //                 ProductDataModel.New("tech", "prod1", "item1", "1"),
        //                 ProductDataModel.New("tech", "prod2", "item2", "2"),
        //                 ProductDataModel.New("tech", "prod3", "item3", "3"),
        //                 ProductDataModel.New("tech", "prod4", "item4", "4"),
        //                 ProductDataModel.New("tech", "prod5", "item5", "5")
        //             }
        //         )
        //     );
        // });
    }
}
