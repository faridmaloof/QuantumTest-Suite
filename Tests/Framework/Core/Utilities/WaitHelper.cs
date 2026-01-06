using Microsoft.Playwright;

namespace QuantumTestSuite.Framework.Core.Utilities;

/// <summary>
/// Provides advanced wait operations for UI testing
/// </summary>
public static class WaitHelper
{
    /// <summary>
    /// Waits for a condition to be true with custom timeout and interval
    /// </summary>
    /// <param name="condition">The condition to check</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    /// <param name="intervalMs">Check interval in milliseconds</param>
    /// <param name="errorMessage">Error message if timeout occurs</param>
    public static async Task<bool> WaitForConditionAsync(
        Func<Task<bool>> condition,
        int timeoutMs = TestTimeouts.Default,
        int intervalMs = TestTimeouts.RetryInterval,
        string errorMessage = "Condition was not met within timeout")
    {
        var endTime = DateTime.UtcNow.AddMilliseconds(timeoutMs);

        while (DateTime.UtcNow < endTime)
        {
            try
            {
                if (await condition())
                {
                    return true;
                }
            }
            catch
            {
                // Ignore exceptions during polling
            }

            await Task.Delay(intervalMs);
        }

        throw new TimeoutException(errorMessage);
    }

    /// <summary>
    /// Waits for a synchronous condition to be true with custom timeout
    /// </summary>
    /// <param name="condition">The synchronous condition to check</param>
    /// <param name="timeout">Timeout as TimeSpan</param>
    /// <param name="errorMessage">Error message if timeout occurs</param>
    public static async Task<bool> WaitForConditionAsync(
        Func<bool> condition,
        TimeSpan timeout,
        string errorMessage = "Condition was not met within timeout")
    {
        return await WaitForConditionAsync(
            () => Task.FromResult(condition()),
            (int)timeout.TotalMilliseconds,
            TestTimeouts.RetryInterval,
            errorMessage);
    }

    /// <summary>
    /// Waits for an element to be visible
    /// </summary>
    /// <param name="page">The page instance</param>
    /// <param name="selector">Element selector</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    public static async Task WaitForVisibleAsync(
        IPage page,
        string selector,
        int timeoutMs = TestTimeouts.Default)
    {
        await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeoutMs
        });
    }

    /// <summary>
    /// Waits for an element to be hidden
    /// </summary>
    /// <param name="page">The page instance</param>
    /// <param name="selector">Element selector</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    public static async Task WaitForHiddenAsync(
        IPage page,
        string selector,
        int timeoutMs = TestTimeouts.Default)
    {
        await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Hidden,
            Timeout = timeoutMs
        });
    }

    /// <summary>
    /// Waits for network to be idle
    /// </summary>
    /// <param name="page">The page instance</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    public static async Task WaitForNetworkIdleAsync(
        IPage page,
        int timeoutMs = TestTimeouts.Default)
    {
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions
        {
            Timeout = timeoutMs
        });
    }
}
