using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Actors;
using QuantumTestSuite.Core.Config;

namespace QuantumTestSuite.UI.Screenplay.Tasks;

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
