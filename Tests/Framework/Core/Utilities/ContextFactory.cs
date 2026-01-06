using Microsoft.Playwright;
using QuantumTestSuite.Framework.Core.Config;

namespace QuantumTestSuite.Framework.Core.Utilities;

/// <summary>
/// Factory for creating Playwright contexts with video and screenshot configuration
/// </summary>
public static class ContextFactory
{
    /// <summary>
    /// Creates a new browser context with configured video and screenshot options
    /// </summary>
    /// <param name="browser">Browser instance</param>
    /// <param name="settings">Application settings</param>
    /// <returns>Configured browser context</returns>
    public static async Task<IBrowserContext> CreateContextAsync(IBrowser browser, AppSettings settings)
    {
        var options = new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            IgnoreHTTPSErrors = true
        };

        // Configure video recording if enabled
        if (settings.Playwright.VideoEnabled)
        {
            options.RecordVideoDir = Path.Combine(settings.Allure.Directory, "videos");
            options.RecordVideoSize = new RecordVideoSize { Width = 1920, Height = 1080 };
        }

        return await browser.NewContextAsync(options);
    }

    /// <summary>
    /// Creates a new page within a context
    /// </summary>
    /// <param name="context">Browser context</param>
    /// <returns>New page instance</returns>
    public static async Task<IPage> CreatePageAsync(IBrowserContext context)
    {
        return await context.NewPageAsync();
    }
}
