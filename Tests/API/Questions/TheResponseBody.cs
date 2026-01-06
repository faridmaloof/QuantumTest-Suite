using QuantumTestSuite.UI.Screenplay.Abilities;
using System.Text.Json;

namespace QuantumTestSuite.API.Questions;

/// <summary>
/// Question to retrieve and deserialize the API response body
/// </summary>
/// <typeparam name="T">The type to deserialize the response into</typeparam>
public class TheResponseBody<T> : IApiQuestion<T?>
{
    private readonly JsonSerializerOptions? _options;

    private TheResponseBody(JsonSerializerOptions? options = null)
    {
        _options = options;
    }

    /// <summary>
    /// Creates a question about the response body
    /// </summary>
    public static TheResponseBody<T> Content => new();

    /// <summary>
    /// Creates a question with custom JSON serialization options
    /// </summary>
    public static TheResponseBody<T> WithOptions(JsonSerializerOptions options) => new(options);

    public async Task<T?> AnsweredBy(UI.Screenplay.Actors.Actor actor)
    {
        var apiAbility = actor.Using<CallApiEndpoint>();
        var response = apiAbility.GetLastResponse();
        
        if (response == null)
            return default;

        var content = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<T>(content, _options);
    }
}
