using Newtonsoft.Json;

namespace Demo.MiniProducts.Api.Extensions;

public static class JsonExtensions
{
    public static async ValueTask<T> ToModel<T>(this Stream stream) where T : class, new()
    {
        using (var reader = new StreamReader(stream))
        {
            var content = await reader.ReadToEndAsync();
            var record = JsonConvert.DeserializeObject<T>(content, new JsonSerializerSettings
            {
                Error = (_, args) => args.ErrorContext.Handled = true
            });

            return record ?? new T();
        }
    }
}