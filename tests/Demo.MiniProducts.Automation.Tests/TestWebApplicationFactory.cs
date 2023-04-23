using Microsoft.AspNetCore.Mvc.Testing;

namespace Demo.MiniProducts.Automation.Tests;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    // TODO: register test services as needed.
}