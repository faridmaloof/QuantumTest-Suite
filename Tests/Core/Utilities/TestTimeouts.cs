namespace QuantumTestSuite.Core.Utilities;

/// <summary>
/// Centralized timeout constants for test execution
/// </summary>
public static class TestTimeouts
{
    /// <summary>
    /// Default timeout for most operations (30 seconds)
    /// </summary>
    public const int Default = 30000;

    /// <summary>
    /// Short timeout for quick operations (5 seconds)
    /// </summary>
    public const int Short = 5000;

    /// <summary>
    /// Long timeout for slow operations (60 seconds)
    /// </summary>
    public const int Long = 60000;

    /// <summary>
    /// Extended timeout for very slow operations (2 minutes)
    /// </summary>
    public const int Extended = 120000;

    /// <summary>
    /// Retry interval for polling operations (500ms)
    /// </summary>
    public const int RetryInterval = 500;
}
