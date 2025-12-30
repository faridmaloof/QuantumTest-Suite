using Microsoft.Playwright;
using QuantumTestSuite.Core.Config;

namespace QuantumTestSuite.UI.Screenplay.Abilities;

/// <summary>
/// Ability to call API endpoints directly without going through the UI.
/// Useful for setup, teardown, or hybrid UI+API test scenarios.
/// </summary>
/// <example>
/// var response = await actor.Using&lt;CallApiEndpoint&gt;()
///     .GetAsync&lt;User&gt;("/api/users/1");
/// </example>
public class CallApiEndpoint : IAbility
{
    private readonly IAPIRequestContext _apiContext;
    private readonly AppSettings _settings;

    public CallApiEndpoint(IAPIRequestContext apiContext, AppSettings settings)
    {
        _apiContext = apiContext ?? throw new ArgumentNullException(nameof(apiContext));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    /// <summary>
    /// Perform a GET request to the specified endpoint.
    /// </summary>
    /// <typeparam name="T">Expected response type</typeparam>
    /// <param name="endpoint">The API endpoint path</param>
    /// <param name="baseUrl">Optional base URL (uses settings if not provided)</param>
    /// <returns>Deserialized response</returns>
    public async Task<T?> GetAsync<T>(string endpoint, string? baseUrl = null) where T : class
    {
        var url = CombineUrl(baseUrl, endpoint);
        var response = await _apiContext.GetAsync(url);
        
        if (!response.Ok)
        {
            throw new HttpRequestException(
                $"GET request to {url} failed with status {response.Status}");
        }

        var json = await response.TextAsync();
        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// Perform a POST request with JSON body.
    /// </summary>
    /// <typeparam name="TRequest">Request body type</typeparam>
    /// <typeparam name="TResponse">Expected response type</typeparam>
    /// <param name="endpoint">The API endpoint path</param>
    /// <param name="body">Request body object</param>
    /// <param name="baseUrl">Optional base URL</param>
    /// <returns>Deserialized response</returns>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest body,
        string? baseUrl = null)
        where TRequest : class
        where TResponse : class
    {
        var url = CombineUrl(baseUrl, endpoint);
        var json = System.Text.Json.JsonSerializer.Serialize(body);
        
        var response = await _apiContext.PostAsync(url, new APIRequestContextOptions
        {
            Data = json,
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json"
            }
        });

        if (!response.Ok)
        {
            throw new HttpRequestException(
                $"POST request to {url} failed with status {response.Status}");
        }

        var responseJson = await response.TextAsync();
        return System.Text.Json.JsonSerializer.Deserialize<TResponse>(responseJson);
    }

    /// <summary>
    /// Perform a raw HTTP request with full control.
    /// </summary>
    /// <param name="method">HTTP method (GET, POST, PUT, DELETE, etc.)</param>
    /// <param name="endpoint">The API endpoint path</param>
    /// <param name="options">Playwright API request options</param>
    /// <returns>API response</returns>
    public async Task<IAPIResponse> RequestAsync(
        string method,
        string endpoint,
        APIRequestContextOptions? options = null)
    {
        var url = CombineUrl(null, endpoint);
        return await _apiContext.FetchAsync(url, new APIRequestContextOptions
        {
            Method = method,
            Data = options?.Data,
            Headers = options?.Headers,
            Params = options?.Params
        });
    }

    private string CombineUrl(string? baseUrl, string endpoint)
    {
        baseUrl ??= _settings.Apis.RestfulBooker; // Default base URL
        return new Uri(new Uri(baseUrl), endpoint).ToString();
    }

    public Task CleanupAsync()
    {
        // API context is managed by TestHooks, no cleanup needed here
        return Task.CompletedTask;
    }
}
