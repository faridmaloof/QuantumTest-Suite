namespace QuantumTestSuite.Framework.UI.Screenplay.Questions;

/// <summary>
/// Question to count the number of elements matching a selector
/// </summary>
public class TheCount : IQuestion<int>
{
    private readonly string _selector;

    private TheCount(string selector) 
        => _selector = selector;

    /// <summary>
    /// Creates a question about the count of elements
    /// </summary>
    /// <param name="selector">CSS selector for the elements</param>
    /// <returns>A question that returns the count of matching elements</returns>
    public static TheCount Of(string selector) 
        => new(selector);

    public async Task<int> AnsweredBy(Actors.Actor actor) 
        => await actor.Page.Locator(_selector).CountAsync();
}
