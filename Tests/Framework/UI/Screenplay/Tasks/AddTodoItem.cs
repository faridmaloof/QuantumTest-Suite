using QuantumTestSuite.Framework.UI.Pages;
using QuantumTestSuite.Framework.UI.Screenplay.Actors;
using QuantumTestSuite.Framework.Core.Config;

namespace QuantumTestSuite.Framework.UI.Screenplay.Tasks;

/// <summary>
/// Task to add a new todo item to the list
/// </summary>
public class AddTodoItem(string todoText, AppSettings? settings = null) : ITask
{
    private readonly string _todoText = todoText;
    private readonly AppSettings? _settings = settings;

    public static AddTodoItem With(string text) => new(text);

    public async Task ExecuteAsync(Actor actor)
    {
        var page = new PlaywrightDemoPage(actor.Page, _settings);
        await page.AddTodoAsync(_todoText);
    }
}
