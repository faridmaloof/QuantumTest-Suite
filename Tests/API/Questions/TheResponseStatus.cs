using System.Net;
using QuantumTestSuite.UI.Screenplay.Abilities;

namespace QuantumTestSuite.API.Questions;

/// <summary>
/// Question to retrieve the HTTP status code of the last API response
/// </summary>
public class TheResponseStatus : IApiQuestion<HttpStatusCode>
{
    private static readonly TheResponseStatus _instance = new();

    private TheResponseStatus() { }

    /// <summary>
    /// Gets the response status code question
    /// </summary>
    public static TheResponseStatus Code => _instance;

    public async Task<HttpStatusCode> AnsweredBy(UI.Screenplay.Actors.Actor actor)
    {
        var apiAbility = actor.Using<CallApiEndpoint>();
        var response = apiAbility.GetLastResponse();
        
        return await Task.FromResult(response?.StatusCode ?? HttpStatusCode.InternalServerError);
    }
}
