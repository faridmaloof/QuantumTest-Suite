namespace QuantumTestSuite.UI.Screenplay.Questions;

/// <summary>
/// Question to retrieve the page title
/// </summary>
public class TheTitle : IQuestion<string>
{
    private static readonly TheTitle _instance = new();

    private TheTitle() { }

    /// <summary>
    /// Gets the page title question
    /// </summary>
    public static TheTitle OfThePage
        => _instance;

    public async Task<string> AnsweredBy(Actors.Actor actor)
        => await actor.Page.TitleAsync();
}
