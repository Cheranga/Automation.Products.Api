using System.Net.Http.Json;

namespace Demo.MiniProducts.Automation.Tests;

public static class HttpMethodCaller
{
    public static async Task<HttpResponseMessage> GetAsync(
        HttpClient client,
        string url,
        (string name, string[] values)[]? headers = null
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var requestHeaders = headers?.ToList() ?? new List<(string name, string[] values)>();
        requestHeaders.ForEach(x => request.Headers.Add(x.name, x.values));

        var httpResponse = await client.SendAsync(request);
        return httpResponse;
    }

    public static async Task<HttpResponseMessage> PutAsync(HttpClient client,
        string url,
        Func<JsonContent> contentFunc,
        (string name, string[] values)[]? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, url);
        var requestHeaders = headers?.ToList() ?? new List<(string name, string[] values)>();
        requestHeaders.ForEach(x => request.Headers.Add(x.name, x.values));

        var httpResponse = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Put, url) { Content = contentFunc() }
        );

        return httpResponse;
    }
}
