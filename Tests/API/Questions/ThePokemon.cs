using QuantumTestSuite.API.Models;
using QuantumTestSuite.UI.Screenplay.Abilities;

namespace QuantumTestSuite.API.Questions;

/// <summary>
/// Question to retrieve Pokemon data from the last API response
/// </summary>
public class ThePokemon : IApiQuestion<Pokemon?>
{
    private static readonly ThePokemon _instance = new();

    private ThePokemon() { }

    public static ThePokemon Data => _instance;

    public async Task<Pokemon?> AnsweredBy(UI.Screenplay.Actors.Actor actor)
    {
        var apiAbility = actor.Using<CallApiEndpoint>();
        var response = apiAbility.GetLastResponse();
        
        if (response == null || !response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        return System.Text.Json.JsonSerializer.Deserialize<Pokemon>(content, 
            new System.Text.Json.JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
    }
}
