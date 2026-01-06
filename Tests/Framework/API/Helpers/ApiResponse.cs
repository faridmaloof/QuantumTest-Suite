using System.Text.Json;
using Microsoft.Playwright;

namespace QuantumTestSuite.Framework.API.Helpers;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Body { get; set; } = string.Empty;
    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    public T? Data { get; set; }
}

public static class ApiResponseMapper
{
    public static async Task<ApiResponse<T>> FromAsync<T>(IAPIResponse response)
    {
        var body = await response.TextAsync();
        var headers = response.Headers.ToDictionary(k => k.Key, v => v.Value);

        T? data = default;
        try
        {
            data = JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            // Ignore deserialization errors for flexible samples.
        }

        return new ApiResponse<T>
        {
            StatusCode = (int)response.Status,
            Body = body,
            Headers = headers,
            Data = data
        };
    }
}
