using System.Text.Json;
using Microsoft.Extensions.Configuration;
using DotNetEnv;

namespace QuantumTestSuite.Core.Config;

public static class ConfigManager
{
    private static readonly Lazy<AppSettings> CachedSettings = new(LoadSettings);

    public static AppSettings Settings => CachedSettings.Value;

    private static AppSettings LoadSettings()
    {
        // Configuration Priority (highest to lowest):
        // 1. Environment Variables (e.g., CI/CD, manual export)
        // 2. User Secrets (dotnet user-secrets)
        // 3. .env.{TEST_ENVIRONMENT} file
        // 4. .env file (base)
        // 5. appsettings.json (defaults/structure)

        // Load .env files first (lower priority)
        LoadEnvFiles();

        // Build configuration with all sources
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false) // Defaults
            .AddUserSecrets<AppSettings>(optional: true)  // User Secrets (if configured)
            .AddEnvironmentVariables();                   // Highest priority

        var configuration = builder.Build();
        var settings = new AppSettings();

        // Bind from all sources (respecting priority order)
        BindSettingsFromEnvironment(settings);

        return settings;
    }

    private static void LoadEnvFiles()
    {
        var baseDir = Directory.GetCurrentDirectory();
        var environment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") 
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
            ?? "local";

        // Load .env files in order (each overrides previous)
        var envFiles = new[]
        {
            Path.Combine(baseDir, ".env"),                          // Base .env
            Path.Combine(baseDir, $".env.{environment.ToLower()}") // Environment-specific
        };

        foreach (var envFile in envFiles)
        {
            if (File.Exists(envFile))
            {
                Env.Load(envFile, new LoadOptions(
                    setEnvVars: true,        // Set as environment variables
                    clobberExistingVars: true // Override existing
                ));
                Console.WriteLine($"[ConfigManager] Loaded: {Path.GetFileName(envFile)}");
            }
        }
    }

    private static void BindSettingsFromEnvironment(AppSettings settings)
    {
        // Environment
        settings.Env = GetEnvVar("TEST_ENVIRONMENT") ?? GetEnvVar("DOTNET_ENVIRONMENT") ?? "local";

        // Playwright Settings
        settings.Playwright.Headless = GetBoolEnvVar("PLAYWRIGHT_HEADLESS", true);
        settings.Playwright.Browser = GetEnvVar("PLAYWRIGHT_BROWSER") ?? "chromium";
        settings.Playwright.BaseUrl = GetEnvVar("PLAYWRIGHT_BASE_URL") ?? "https://www.saucedemo.com/";
        settings.Playwright.SlowMo = GetIntEnvVar("PLAYWRIGHT_SLOWMO", 0);
        settings.Playwright.VideoEnabled = GetBoolEnvVar("PLAYWRIGHT_VIDEO_ENABLED", false);

        // Screenshot Options
        settings.Playwright.ScreenshotOptions.BeforeStep = GetBoolEnvVar("SCREENSHOT_BEFORE_STEP", false);
        settings.Playwright.ScreenshotOptions.AfterStep = GetBoolEnvVar("SCREENSHOT_AFTER_STEP", false);
        settings.Playwright.ScreenshotOptions.OnFailure = GetBoolEnvVar("SCREENSHOT_ON_FAILURE", true);

        // API URLs
        settings.Apis.HttpBin = GetEnvVar("API_HTTPBIN") ?? "https://httpbin.org";
        settings.Apis.RestfulBooker = GetEnvVar("API_RESTFUL_BOOKER") ?? "https://restful-booker.herokuapp.com";
        settings.Apis.GitHub = GetEnvVar("API_GITHUB") ?? "https://api.github.com";
        settings.Apis.GhUsersSearchUi = GetEnvVar("API_GH_USERS_SEARCH_UI") ?? "https://gh-users-search.netlify.app/";

        // User Credentials
        settings.Users.SauceDemo.Username = GetEnvVar("SAUCEDEMO_USERNAME") ?? "";
        settings.Users.SauceDemo.Password = GetEnvVar("SAUCEDEMO_PASSWORD") ?? "";

        // Allure Settings
        settings.Allure.Directory = GetEnvVar("ALLURE_RESULTS_DIRECTORY") ?? "Reports/AllureResults";
        settings.Allure.CleanupCycle = GetIntEnvVar("ALLURE_CLEANUP_CYCLE", 1);
    }

    private static string? GetEnvVar(string key) => Environment.GetEnvironmentVariable(key);

    private static bool GetBoolEnvVar(string key, bool defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    private static int GetIntEnvVar(string key, int defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    public static string? GetEnvironmentVariable(string key) => Environment.GetEnvironmentVariable(key);

    public static string ToJson(object instance) => JsonSerializer.Serialize(instance, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    });
}
