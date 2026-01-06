using Allure.Net.Commons;
using Microsoft.Playwright;
using NUnit.Framework;
using QuantumTestSuite.Framework.Core.Config;
using QuantumTestSuite.Framework.Core.Context;
using QuantumTestSuite.Framework.Core.Reporting;
using QuantumTestSuite.Framework.UI.Drivers;
using Reqnroll;

namespace QuantumTestSuite.Framework.Core.Hooks;

[Binding]
public class TestHooks
{
    private readonly ScenarioContext _scenarioContext;
    private readonly AppSettings _settings;

    [BeforeTestRun(Order = -100)]
    public static void EnsureAllureDirectories()
    {
        var baseDir = AppContext.BaseDirectory;
        var settings = ConfigManager.Settings;
        var allureDir = Path.Combine(baseDir, settings.Allure.Directory);

        Directory.CreateDirectory(allureDir);
        
        // Create videos directory if video recording is enabled
        if (settings.Playwright.VideoEnabled)
        {
            var videosDir = Path.Combine(allureDir, "videos");
            Directory.CreateDirectory(videosDir);
        }

        // Make sure Allure picks the config copied to output
        Environment.SetEnvironmentVariable("ALLURE_CONFIG", "allureConfig.json");
        
        // Write Allure metadata files for better reporting
        AllureEnvironmentWriter.WriteEnvironmentInfo(allureDir);
        AllureExecutorWriter.WriteExecutorInfo(allureDir);
        AllureCategoriesWriter.WriteCategoriesFile(allureDir);
    }

    public TestHooks(ScenarioContext scenarioContext, AppSettings settings)
    {
        _scenarioContext = scenarioContext;
        _settings = settings;
    }

    [BeforeScenario(Order = 0)]
    public void RegisterSettings()
    {
        _scenarioContext["settings"] = _settings;
    }

    [BeforeScenario("api", Order = 1)]
    public void InitializeApiContext()
    {
        var apiContext = new ApiTestContext();
        _scenarioContext.Set(apiContext);
    }

    [BeforeScenario("ui", Order = 1)]
    public void InitializeUiContext()
    {
        var uiContext = new UiTestContext();
        _scenarioContext.Set(uiContext);
    }

    // Screenshot capture is now handled directly in UiStepBindingsBase.Execute*Async methods
    // This ensures screenshots are attached to the step itself, not in hooks
    
    //[BeforeStep("ui")]
    //public async Task CaptureScreenshotBeforeStep()
    //{
    //    if (!_scenarioContext.TryGetValue<UiTestContext>(out var context) || context.Page == null)
    //        return;
    //
    //    await AllureHelper.CaptureScreenshotIfConfiguredAsync(
    //        context.Page,
    //        _scenarioContext.StepContext.StepInfo.Text,
    //        ScreenshotTiming.BeforeStep);
    //}

    //[AfterStep("ui")]
    //public async Task CaptureScreenshotAfterStep()
    //{
    //    if (_scenarioContext.TestError != null)
    //        return; // Skip if failed, OnFailure will handle it
    //
    //    if (!_scenarioContext.TryGetValue<UiTestContext>(out var context) || context.Page == null)
    //        return;
    //
    //    await AllureHelper.CaptureScreenshotIfConfiguredAsync(
    //        context.Page,
    //        _scenarioContext.StepContext.StepInfo.Text,
    //        ScreenshotTiming.AfterStep);
    //}

    [AfterStep]
    public async Task CaptureArtifactsOnFailure()
    {
        if (_scenarioContext.TestError == null)
            return;

        // Try UI context first
        if (_scenarioContext.TryGetValue<UiTestContext>(out var uiContext) && uiContext.Page != null)
        {
            await AllureHelper.CaptureScreenshotIfConfiguredAsync(
                uiContext.Page,
                _scenarioContext.StepContext.StepInfo.Text,
                ScreenshotTiming.OnFailure);
        }

        // Try API context
        if (_scenarioContext.TryGetValue<ApiTestContext>(out var apiContext) && !string.IsNullOrWhiteSpace(apiContext.LastResponse))
        {
            AllureHelper.AttachJson("last-api-response", apiContext.LastResponse);
        }
    }

    [AfterScenario("ui")]
    public async Task CleanupUiAsync()
    {
        if (!_scenarioContext.TryGetValue<UiTestContext>(out var context))
            return;

        // Save video path before closing (video is saved when context closes)
        string? videoPath = null;
        if (_settings.Playwright.VideoEnabled && context.Page?.Video != null)
        {
            try
            {
                videoPath = await context.Page.Video.PathAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get video path: {ex.Message}");
            }
        }

        // Close page first
        if (context.Page != null)
        {
            await context.Page.CloseAsync();
        }

        // Close context (this finalizes video encoding)
        if (context.BrowserContext != null)
        {
            await context.BrowserContext.CloseAsync();
        }

        // Now attach the video after context is closed and video is fully saved
        if (!string.IsNullOrEmpty(videoPath))
        {
            await AllureHelper.AttachVideoFromPathAsync(videoPath);
        }

        if (context.Browser != null)
        {
            await context.Browser.CloseAsync();
        }

        if (context.Playwright != null)
        {
            context.Playwright.Dispose();
        }
    }

    [AfterScenario("api")]
    public void CleanupApi()
    {
        if (!_scenarioContext.TryGetValue<ApiTestContext>(out var context))
            return;

        if (context.Playwright != null)
        {
            context.Playwright.Dispose();
        }
    }
}
