using System.Net.Http.Json;

namespace Demo.MiniProducts.Automation.Tests;

public abstract class TestBase : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    protected TestBase(TestWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(
        string url,
        T data,
        Func<(string name, string value)[]> headers
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        var requestHeaders = headers()?.ToList() ?? new List<(string name, string value)>();
        requestHeaders.ForEach(x => request.Headers.Add(x.name, x.value));

        var httpResponse = await _client.SendAsync(
            new HttpRequestMessage(HttpMethod.Post, url) { Content = JsonContent.Create(data) }
        );

        return httpResponse;
    }

    protected async Task<HttpResponseMessage> GetAsync<T>(
        string url,
        Func<(string name, string value)[]> headers
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var requestHeaders = headers()?.ToList() ?? new List<(string name, string value)>();
        requestHeaders.ForEach(x => request.Headers.Add(x.name, x.value));

        var httpResponse = await _client.SendAsync(request);
        return httpResponse;
    }
}
