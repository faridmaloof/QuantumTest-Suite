namespace QuantumTestSuite.UI.Screenplay.Abilities;

/// <summary>
/// Ability to remember and recall data throughout a scenario.
/// This provides a type-safe way to store and retrieve test data between steps.
/// </summary>
/// <example>
/// actor.Using&lt;RememberData&gt;().Remember("username", "admin");
/// var username = actor.Using&lt;RememberData&gt;().Recall&lt;string&gt;("username");
/// </example>
public class RememberData : IAbility
{
    private readonly Dictionary<string, object> _memory = new();

    /// <summary>
    /// Store a value with a key for later retrieval.
    /// </summary>
    /// <typeparam name="T">The type of value to store</typeparam>
    /// <param name="key">Unique identifier for this value</param>
    /// <param name="value">The value to remember</param>
    public void Remember<T>(string key, T value) where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        _memory[key] = value;
    }

    /// <summary>
    /// Retrieve a previously stored value.
    /// </summary>
    /// <typeparam name="T">The expected type of the value</typeparam>
    /// <param name="key">The key used when storing the value</param>
    /// <returns>The stored value, or null if not found</returns>
    public T? Recall<T>(string key) where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        if (!_memory.TryGetValue(key, out var value))
        {
            return null;
        }

        if (value is not T typedValue)
        {
            throw new InvalidCastException(
                $"Value stored with key '{key}' is of type {value.GetType().Name}, " +
                $"but {typeof(T).Name} was requested.");
        }

        return typedValue;
    }

    /// <summary>
    /// Check if a value with the given key exists in memory.
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if the key exists, false otherwise</returns>
    public bool Has(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _memory.ContainsKey(key);
    }

    /// <summary>
    /// Remove a value from memory.
    /// </summary>
    /// <param name="key">The key of the value to forget</param>
    /// <returns>True if the value was removed, false if it didn't exist</returns>
    public bool Forget(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _memory.Remove(key);
    }

    /// <summary>
    /// Clear all stored values.
    /// </summary>
    public void ForgetAll() 
        => _memory.Clear();

    /// <summary>
    /// Get all stored keys.
    /// </summary>
    public IReadOnlyCollection<string> Keys => _memory.Keys;

    /// <summary>
    /// Cleanup: Clear all stored data.
    /// </summary>
    public Task CleanupAsync()
    {
        ForgetAll();
        return Task.CompletedTask;
    }
}
