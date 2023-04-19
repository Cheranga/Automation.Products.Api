using System.Diagnostics.CodeAnalysis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Demo.MiniProducts.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class JsonExtensions
{
    public static async ValueTask<T> ToModel<T>(this Stream stream) where T : new()
    {
        using (var reader = new StreamReader(stream))
        {
            var content = await reader.ReadToEndAsync();
            var record = JsonSerializer.Deserialize<T>(content);

            return record ?? new T();
        }
    }

    public static Func<string> ToStringFunc<T>(this T model) =>
        () => JsonSerializer.Serialize(model);
}