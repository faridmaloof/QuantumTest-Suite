using QuantumTestSuite.Framework.UI.Pages;
using QuantumTestSuite.Framework.UI.Screenplay.Actors;
using QuantumTestSuite.Framework.Core.Config;

namespace QuantumTestSuite.Framework.UI.Screenplay.Tasks;

/// <summary>
/// Task to navigate to the Playwright demo page
/// </summary>
public class NavigateToPlaywrightDemo(AppSettings? settings = null) : ITask
{
    private readonly AppSettings? _settings = settings;

    public static NavigateToPlaywrightDemo Page => new();

    public async Task ExecuteAsync(Actor actor)
    {
        var page = new PlaywrightDemoPage(actor.Page, _settings);
        await page.NavigateAsync();
    }
}
