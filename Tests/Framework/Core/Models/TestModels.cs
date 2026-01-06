namespace QuantumTestSuite.Framework.Core.Models;

/// <summary>
/// Represents a user entity from the database.
/// Used for test scenarios that require user data.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string Role { get; set; } = "User";

    public string FullName => $"{FirstName} {LastName}".Trim();

    public override string ToString() => $"{Username} ({Email}) - Active: {Active}";
}

/// <summary>
/// Represents a snapshot of test data retrieved at a specific point in time.
/// Useful for tracking when and what data was used in a test.
/// </summary>
public class TestDataSnapshot<T> where T : class
{
    public T Data { get; set; } = default!;
    public DateTime RetrievedAt { get; set; } = DateTime.Now;
    public string Source { get; set; } = "Unknown";
    public Dictionary<string, object> Metadata { get; set; } = new();

    public void AddMetadata(string key, object value)
    {
        Metadata[key] = value;
    }

    public override string ToString() =>
        $"{typeof(T).Name} from {Source} at {RetrievedAt:yyyy-MM-dd HH:mm:ss}";
}

/// <summary>
/// Database configuration for test scenarios.
/// </summary>
public class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Provider { get; set; } = "SqlServer"; // SqlServer, PostgreSQL, MySQL, etc.
    public int CommandTimeout { get; set; } = 30;
    public bool EnableRetry { get; set; } = true;
    public int MaxRetryCount { get; set; } = 3;
}

/// <summary>
/// Represents the result of a database query with metadata.
/// </summary>
/// <typeparam name="T">The type of data returned</typeparam>
public class QueryResult<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public string Query { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    public bool HasData => Data?.Any() == true;
    public T? First => Data != null && Data.Any() ? Data.First() : default;

    public override string ToString() =>
        Success
            ? $"Query returned {TotalCount} rows in {ExecutionTime.TotalMilliseconds:F2}ms"
            : $"Query failed: {ErrorMessage}";
}
