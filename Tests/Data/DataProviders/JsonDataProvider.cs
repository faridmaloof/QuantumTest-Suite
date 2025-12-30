using System.Text.Json;

namespace QuantumTestSuite.Data.DataProviders;

public static class JsonDataProvider
{
    public static T Read<T>(string relativePath)
    {
        var path = Path.Combine(AppContext.BaseDirectory, relativePath);
        var content = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
}
