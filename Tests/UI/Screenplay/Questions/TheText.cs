using Microsoft.Playwright;

namespace QuantumTestSuite.UI.Screenplay.Questions;

/// <summary>
/// Question to retrieve text content from an element on the page
/// </summary>
public class TheText : IQuestion<string>
{
    private readonly string _selector;
    private readonly bool _innerText;

    private TheText(string selector, bool innerText = true)
    {
        _selector = selector;
        _innerText = innerText;
    }

    /// <summary>
    /// Creates a question about the text of an element
    /// </summary>
    /// <param name="selector">CSS selector for the element</param>
    /// <returns>A question that returns the element's text</returns>
    public static TheText Of(string selector) 
        => new(selector, innerText: true);

    /// <summary>
    /// Creates a question about the inner text of an element
    /// </summary>
    public static TheText InnerTextOf(string selector) 
        => new(selector, innerText: true);

    /// <summary>
    /// Creates a question about the text content of an element
    /// </summary>
    public static TheText ContentOf(string selector) 
        => new(selector, innerText: false);

    public async Task<string> AnsweredBy(Actors.Actor actor)
    {
        var element = await actor.Page.WaitForSelectorAsync(_selector);
        
        if (element == null)
            return string.Empty;

        return _innerText 
            ? await element.InnerTextAsync() 
            : await element.TextContentAsync() ?? string.Empty;
    }
}
