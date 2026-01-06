using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuantumTestSuite.Framework.Core.Reporting;

/// <summary>
/// Writes enhanced categories for Allure reports
/// </summary>
public static class AllureCategoriesWriter
{
    public static void WriteCategoriesFile(string allureResultsPath)
    {
        var categories = GetCategories();
        var categoriesFilePath = Path.Combine(allureResultsPath, "categories.json");
        
        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        
        var json = JsonSerializer.Serialize(categories, options);
        
        Directory.CreateDirectory(Path.GetDirectoryName(categoriesFilePath)!);
        File.WriteAllText(categoriesFilePath, json);
    }
    
    private static List<AllureCategory> GetCategories()
    {
        return new List<AllureCategory>
        {
            // Product bugs - assertion failures
            new AllureCategory
            {
                Name = "🐛 Product Bugs",
                Description = "Test failures due to application defects",
                MatchedStatuses = new[] { "failed" },
                MessageRegex = ".*(AssertionException|AssertionError|Expected.*but was).*"
            },
            
            // API issues
            new AllureCategory
            {
                Name = "🌐 API Issues",
                Description = "API-related failures (HTTP errors, timeouts, invalid responses)",
                MatchedStatuses = new[] { "broken", "failed" },
                MessageRegex = ".*(HttpRequestException|WebException|API|HTTP|status code|endpoint).*"
            },
            
            // UI/Browser issues
            new AllureCategory
            {
                Name = "🖥️ UI/Browser Issues",
                Description = "Browser automation failures (locators, timeouts, page errors)",
                MatchedStatuses = new[] { "broken", "failed" },
                MessageRegex = ".*(PlaywrightException|TimeoutError|Locator|ElementNotFound|SelectorError).*"
            },
            
            // Infrastructure/Environment issues
            new AllureCategory
            {
                Name = "🔧 Infrastructure Issues",
                Description = "Environment, network, or system-related failures",
                MatchedStatuses = new[] { "broken" },
                MessageRegex = ".*(TimeoutException|WebException|SocketException|IOException|ConnectionException|NetworkException).*"
            },
            
            // Configuration issues
            new AllureCategory
            {
                Name = "⚙️ Configuration Errors",
                Description = "Missing or invalid configuration",
                MatchedStatuses = new[] { "broken" },
                MessageRegex = ".*(ConfigurationException|FileNotFoundException|appsettings|configuration|.env).*"
            },
            
            // Test data issues
            new AllureCategory
            {
                Name = "📊 Test Data Issues",
                Description = "Problems with test data or factories",
                MatchedStatuses = new[] { "broken", "failed" },
                MessageRegex = ".*(NullReferenceException|ArgumentNullException|ArgumentException|DataException|Factory|TestData).*"
            },
            
            // Authentication/Authorization
            new AllureCategory
            {
                Name = "🔐 Authentication Issues",
                Description = "Login, authentication, or authorization failures",
                MatchedStatuses = new[] { "failed", "broken" },
                MessageRegex = ".*(Authentication|Authorization|Login|Unauthorized|401|403|Forbidden).*"
            },
            
            // Ignored tests
            new AllureCategory
            {
                Name = "⏭️ Ignored Tests",
                Description = "Tests that were skipped or ignored",
                MatchedStatuses = new[] { "skipped" },
                MessageRegex = ".*"
            }
        };
    }
}

public class AllureCategory
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("messageRegex")]
    public string? MessageRegex { get; set; }
    
    [JsonPropertyName("traceRegex")]
    public string? TraceRegex { get; set; }
    
    [JsonPropertyName("matchedStatuses")]
    public string[]? MatchedStatuses { get; set; }
}
