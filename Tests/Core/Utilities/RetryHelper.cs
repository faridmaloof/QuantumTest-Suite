namespace QuantumTestSuite.Core.Utilities;

/// <summary>
/// Provides retry logic for flaky operations
/// </summary>
public static class RetryHelper
{
    /// <summary>
    /// Executes an action with retry logic
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <param name="maxAttempts">Maximum number of attempts</param>
    /// <param name="delayMilliseconds">Delay between attempts</param>
    /// <param name="onRetry">Optional callback on retry</param>
    public static async Task ExecuteAsync(
        Func<Task> action,
        int maxAttempts = 3,
        int delayMilliseconds = 1000,
        Action<int, Exception>? onRetry = null)
    {
        Exception? lastException = null;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
                
                if (attempt < maxAttempts)
                {
                    onRetry?.Invoke(attempt, ex);
                    await Task.Delay(delayMilliseconds);
                }
            }
        }

        throw new InvalidOperationException(
            $"Operation failed after {maxAttempts} attempts. Last error: {lastException?.Message}",
            lastException);
    }

    /// <summary>
    /// Executes a function with retry logic and returns result
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="func">The function to execute</param>
    /// <param name="maxAttempts">Maximum number of attempts</param>
    /// <param name="delayMilliseconds">Delay between attempts</param>
    /// <param name="onRetry">Optional callback on retry</param>
    /// <returns>Result of the function</returns>
    public static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> func,
        int maxAttempts = 3,
        int delayMilliseconds = 1000,
        Action<int, Exception>? onRetry = null)
    {
        Exception? lastException = null;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                lastException = ex;
                
                if (attempt < maxAttempts)
                {
                    onRetry?.Invoke(attempt, ex);
                    await Task.Delay(delayMilliseconds);
                }
            }
        }

        throw new InvalidOperationException(
            $"Operation failed after {maxAttempts} attempts. Last error: {lastException?.Message}",
            lastException);
    }
}
