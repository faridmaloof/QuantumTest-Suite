namespace QuantumTestSuite.Framework.UI.Screenplay.Questions;

/// <summary>
/// Question to retrieve the current URL of the page
/// </summary>
public class TheCurrentUrl : IQuestion<string>
{
    private static readonly TheCurrentUrl _instance = new();

    private TheCurrentUrl() { }

    /// <summary>
    /// Gets the current URL question
    /// </summary>
    public static TheCurrentUrl Value 
        => _instance;

    public async Task<string> AnsweredBy(Actors.Actor actor) 
        => await Task.FromResult(actor.Page.Url);
}
