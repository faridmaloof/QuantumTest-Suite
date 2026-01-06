using Microsoft.Playwright;

namespace QuantumTestSuite.UI.Screenplay.Questions;

/// <summary>
/// Question to check if an element is visible on the page
/// </summary>
public class TheVisibility : IQuestion<bool>
{
    private readonly string _selector;
    private readonly int _timeout;

    private TheVisibility(string selector, int timeout = 5000)
    {
        _selector = selector;
        _timeout = timeout;
    }

    /// <summary>
    /// Creates a question about the visibility of an element
    /// </summary>
    /// <param name="selector">CSS selector for the element</param>
    /// <returns>A question that returns true if element is visible</returns>
    public static TheVisibility Of(string selector) => new(selector);

    /// <summary>
    /// Sets a custom timeout for checking visibility
    /// </summary>
    public TheVisibility WithTimeout(int milliseconds) 
        => new(_selector, milliseconds);

    public async Task<bool> AnsweredBy(Actors.Actor actor)
    {
        try
        {
            await actor.Page.WaitForSelectorAsync(_selector, new() 
            { 
                Timeout = _timeout,
                State = WaitForSelectorState.Visible
            });
            return true;
        }
        catch
        {
            return false;
        }
    }
}
