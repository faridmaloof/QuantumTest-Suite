using QuantumTestSuite.Core.Config;

namespace QuantumTestSuite.UI.Screenplay.Abilities;

/// <summary>
/// Ability to read configuration values from AppSettings.
/// Provides a convenient way for Actors to access environment-specific config.
/// </summary>
/// <example>
/// var baseUrl = actor.Using&lt;ReadConfiguration&gt;().GetBaseUrl();
/// var isHeadless = actor.Using&lt;ReadConfiguration&gt;().IsHeadless();
/// </example>
public class ReadConfiguration(AppSettings settings) : IAbility
{
    private readonly AppSettings _settings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <summary>
    /// Get the base URL for UI tests.
    /// </summary>
    public string GetBaseUrl()
        => _settings.Playwright.BaseUrl;

    /// <summary>
    /// Check if headless mode is enabled.
    /// </summary>
    public bool IsHeadless()
        => _settings.Playwright.Headless;

    /// <summary>
    /// Get the configured browser type.
    /// </summary>
    public string GetBrowser()
        => _settings.Playwright.Browser;

    /// <summary>
    /// Check if video recording is enabled.
    /// </summary>
    public bool IsVideoEnabled()
        => _settings.Playwright.VideoEnabled;

    /// <summary>
    /// Get API URL by name.
    /// </summary>
    /// <param name="apiName">API name (HttpBin, RestfulBooker, GitHub, etc.)</param>
    /// <returns>The configured API URL</returns>
    public string GetApiUrl(string apiName)
        => apiName.ToLowerInvariant() switch
    {
        "httpbin" => _settings.Apis.HttpBin,
        "restfulbooker" or "booking" => _settings.Apis.RestfulBooker,
        "github" => _settings.Apis.GitHub,
        "ghusers" or "ghsearch" => _settings.Apis.GhUsersSearchUi,
        _ => throw new ArgumentException($"Unknown API name: {apiName}", nameof(apiName))
    };

    /// <summary>
    /// Get user credentials for a specific service.
    /// </summary>
    /// <param name="service">Service name (e.g., "SauceDemo")</param>
    /// <returns>Tuple of (username, password)</returns>
    public (string Username, string Password) GetCredentials(string service)
    {
        return service.ToLowerInvariant() switch
        {
            "saucedemo" => (_settings.Users.SauceDemo.Username, _settings.Users.SauceDemo.Password),
            _ => throw new ArgumentException($"Unknown service: {service}", nameof(service))
        };
    }

    /// <summary>
    /// Get the current environment name.
    /// </summary>
    public string GetEnvironment()
        => _settings.Env;

    /// <summary>
    /// Get the full AppSettings instance for advanced scenarios.
    /// </summary>
    public AppSettings GetSettings()
        => _settings;

    public static Task CleanupAsync()
        => Task.CompletedTask;
}
