using Microsoft.Playwright;
using QuantumTestSuite.Framework.Core.Config;
using QuantumTestSuite.Framework.Core.Context;
using QuantumTestSuite.Framework.Core.Reporting;
using QuantumTestSuite.Framework.UI.Drivers;
using QuantumTestSuite.Framework.UI.Screenplay.Actors;
using Reqnroll;

namespace QuantumTestSuite.StepBindings.Base;

/// <summary>
/// Base class for UI step bindings with automatic screenshot and video management
/// </summary>
public abstract class UiStepBindingsBase
{
    protected readonly ScenarioContext ScenarioContext;
    protected readonly AppSettings Settings;
    protected UiTestContext Context => ScenarioContext.Get<UiTestContext>();
    protected Actor? Actor;

    protected UiStepBindingsBase(ScenarioContext scenarioContext, AppSettings settings)
    {
        ScenarioContext = scenarioContext;
        Settings = settings;
    }

    /// <summary>
    /// Ensures actor is initialized and page is ready
    /// </summary>
    protected async Task<Actor> EnsureActorAsync(string actorName)
    {
        if (Actor != null && Context.ActorName == actorName)
            return Actor;

        if (Context.Playwright == null || Context.Browser == null || Context.BrowserContext == null || Context.Page == null)
        {
            var (playwright, browser, browserContext, page) = await PlaywrightDriver.LaunchAsync(Settings);
            Context.Playwright = playwright;
            Context.Browser = browser;
            Context.BrowserContext = browserContext;
            Context.Page = page;
        }

        Context.ActorName = actorName;
        Actor = new Actor(actorName, Context.Page!);
        
        return Actor;
    }

    /// <summary>
    /// Executes a UI action with automatic screenshot capture before/after based on configuration
    /// Use this for Given steps (setup actions)
    /// </summary>
    protected async Task ExecuteGivenAsync(Func<Task> action)
    {
        await action();
        
        // Capture screenshot after setup (if configured)
        await CaptureScreenshotAsync(ScreenshotTiming.AfterStep);
    }

    /// <summary>
    /// Executes a UI action with automatic screenshot capture before/after based on configuration
    /// Use this for When steps (actions)
    /// </summary>
    protected async Task ExecuteWhenAsync(Func<Task> action)
    {
        await action();
        
        // Capture screenshot after action (if configured)
        await CaptureScreenshotAsync(ScreenshotTiming.AfterStep);
    }

    /// <summary>
    /// Executes a UI assertion with automatic screenshot capture before/after based on configuration
    /// Use this for Then steps (assertions)
    /// </summary>
    protected async Task ExecuteThenAsync(Func<Task> assertion)
    {
        // Capture screenshot before assertion (if configured)
        await CaptureScreenshotAsync(ScreenshotTiming.BeforeStep);
        
        await assertion();
        
        // Capture screenshot after successful assertion (if configured)
        await CaptureScreenshotAsync(ScreenshotTiming.AfterStep);
    }

    /// <summary>
    /// Captures screenshot if page is available and timing is configured
    /// </summary>
    private async Task CaptureScreenshotAsync(ScreenshotTiming timing)
    {
        if (Context.Page == null)
            return;

        await AllureHelper.CaptureScreenshotIfConfiguredAsync(
            Context.Page,
            ScenarioContext.StepContext.StepInfo.Text,
            timing);
    }

    /// <summary>
    /// Navigates to a page and automatically captures screenshot
    /// </summary>
    protected async Task NavigateToAsync(string url)
    {
        await EnsureActorAsync("QA");
        await Context.Page!.GotoAsync(url);
        await CaptureScreenshotAsync(ScreenshotTiming.AfterStep);
    }

    /// <summary>
    /// Executes a Screenplay task with automatic screenshot after
    /// </summary>
    protected async Task PerformTaskAsync(params ITask[] tasks)
    {
        if (Actor == null)
            throw new InvalidOperationException("Actor not initialized. Call EnsureActorAsync first.");
        
        await Actor.AttemptsTo(tasks);
        await CaptureScreenshotAsync(ScreenshotTiming.AfterStep);
    }
}
