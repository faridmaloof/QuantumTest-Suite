using System.Text.Json;
using Microsoft.Playwright;
using QuantumTestSuite.Framework.Core.Config;
using QuantumTestSuite.Framework.Core.Reporting;

namespace QuantumTestSuite.Framework.API.Helpers;

public static class ApiRequestContextFactory
{
    public static async Task<IAPIRequestContext> CreateAsync(IPlaywright playwright, string baseUrl)
    {
        return await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = baseUrl,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                { "Accept", "application/json" },
                { "User-Agent", "QuantumTestSuite/1.0" }
            }
        });
    }

    public static async Task LogResponseAsync(IAPIResponse response)
    {
        var payload = await response.TextAsync();
        var formatted = TryFormatJson(payload);
        AllureHelper.AttachJson("response", formatted);
    }

    private static string TryFormatJson(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            return body;
        }
    }
}
