namespace QuantumTestSuite.UI.Screenplay.Questions;

/// <summary>
/// Question to retrieve the value of an input element
/// </summary>
public class TheValue : IQuestion<string>
{
    private readonly string _selector;

    private TheValue(string selector) 
        => _selector = selector;

    /// <summary>
    /// Creates a question about the value of an input element
    /// </summary>
    /// <param name="selector">CSS selector for the input element</param>
    /// <returns>A question that returns the input's value</returns>
    public static TheValue Of(string selector) 
        => new(selector);

    public async Task<string> AnsweredBy(Actors.Actor actor)
    {
        var element = await actor.Page.WaitForSelectorAsync(_selector);
        return element != null ? await element.InputValueAsync() : string.Empty;
    }
}
