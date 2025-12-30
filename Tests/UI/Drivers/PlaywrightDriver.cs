using Microsoft.Playwright;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Core.Logging;
using QuantumTestSuite.Core.Utilities;

namespace QuantumTestSuite.UI.Drivers;

public static class PlaywrightDriver
{
    public static async Task<(IPlaywright playwright, IBrowser browser, IBrowserContext context, IPage page)>
        LaunchAsync(AppSettings settings)
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        
        var browser = settings.Playwright.Browser.ToLowerInvariant() switch
        {
            "firefox" => await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = settings.Playwright.Headless,
                SlowMo = settings.Playwright.SlowMo
            }),
            "webkit" => await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = settings.Playwright.Headless,
                SlowMo = settings.Playwright.SlowMo
            }),
            _ => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = settings.Playwright.Headless,
                SlowMo = settings.Playwright.SlowMo
            })
        };

        // Use ContextFactory for consistent context creation with video support
        var context = await ContextFactory.CreateContextAsync(browser, settings);
        
        // Apply base URL
        if (!string.IsNullOrEmpty(settings.Playwright.BaseUrl))
        {
            await context.RouteAsync("**/*", async route =>
            {
                await route.ContinueAsync();
            });
        }

        context.SetDefaultTimeout(TestTimeouts.Default);
        context.SetDefaultNavigationTimeout(TestTimeouts.Default);

        var page = await ContextFactory.CreatePageAsync(context);
        
        if (!string.IsNullOrEmpty(settings.Playwright.BaseUrl))
        {
            page.SetDefaultNavigationTimeout(TestTimeouts.Default);
        }

        ConsoleLogger.Info($"Playwright started with {settings.Playwright.Browser} " +
                          $"(Headless={settings.Playwright.Headless}, Video={settings.Playwright.VideoEnabled})");
        
        return (playwright, browser, context, page);
    }
}
