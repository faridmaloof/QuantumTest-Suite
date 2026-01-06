using QuantumTestSuite.Framework.UI.Locators;

namespace QuantumTestSuite.Framework.UI.Screenplay.Questions;

/// <summary>
/// Question to retrieve all todo item texts from the list
/// </summary>
public class TheTodoItems : IQuestion<List<string>>
{
    private static readonly TheTodoItems _instance = new();

    private TheTodoItems() { }

    public static TheTodoItems Text 
        => _instance;

    public async Task<List<string>> AnsweredBy(Actors.Actor actor)
    {
        var locator = actor.Page.Locator(PlaywrightDemoLocators.TodoItemLabel);
        var count = await locator.CountAsync();
        var texts = new List<string>();

        for (int i = 0; i < count; i++)
        {
            var text = await locator.Nth(i).InnerTextAsync();
            texts.Add(text);
        }

        return texts;
    }
}
