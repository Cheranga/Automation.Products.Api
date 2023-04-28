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
    }
}
