using System.Data;
using System.Diagnostics;
using Dapper;
using QuantumTestSuite.Framework.Core.Models;
using QuantumTestSuite.Framework.Core.Utilities;

namespace QuantumTestSuite.Framework.UI.Screenplay.Abilities;

/// <summary>
/// Ability to access and query databases.
/// Provides a clean interface for Actors to interact with database systems.
/// </summary>
/// <remarks>
/// This ability uses Dapper for database access and supports retry logic for transient failures.
/// Connection management should be handled by the DI container or test hooks.
/// </remarks>
/// <example>
/// var users = await actor.Using&lt;AccessDatabase&gt;()
///     .QueryAsync&lt;User&gt;("SELECT * FROM Users WHERE Active = @Active", new { Active = true });
/// </example>
public class AccessDatabase(IDbConnection connection, DatabaseConfig? config = null) : IAbility
{
    private readonly IDbConnection _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    private readonly DatabaseConfig _config = config ?? new DatabaseConfig();
    private bool _isInitialized;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        // Ensure connection is open
        if (_connection.State != ConnectionState.Open)
        {
            await Task.Run(() => _connection.Open());
        }

        _isInitialized = true;
    }

    /// <summary>
    /// Execute a query and return strongly-typed results.
    /// </summary>
    /// <typeparam name="T">Type of objects to return</typeparam>
    /// <param name="sql">SQL query</param>
    /// <param name="parameters">Query parameters</param>
    /// <returns>Query result with metadata</returns>
    public async Task<QueryResult<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        var result = new QueryResult<T>
        {
            Query = sql,
            Parameters = parameters != null
                ? ConvertToDict(parameters)
                : new Dictionary<string, object>()
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            if (_config.EnableRetry)
            {
                result.Data = (await RetryHelper.ExecuteAsync(
                    async () => (await _connection.QueryAsync<T>(sql, parameters, commandTimeout: _config.CommandTimeout)).ToList(),
                    maxAttempts: _config.MaxRetryCount
                )).ToList();
            }
            else
            {
                result.Data = (await _connection.QueryAsync<T>(sql, parameters, commandTimeout: _config.CommandTimeout)).ToList();
            }

            result.TotalCount = result.Data.Count;
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
        }

        return result;
    }

    /// <summary>
    /// Execute a query and return a single result.
    /// </summary>
    /// <typeparam name="T">Type of object to return</typeparam>
    /// <param name="sql">SQL query</param>
    /// <param name="parameters">Query parameters</param>
    /// <returns>Single object or default(T)</returns>
    public async Task<T?> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        var result = await QueryAsync<T>(sql, parameters);
        return result.First;
    }

    /// <summary>
    /// Execute a command (INSERT, UPDATE, DELETE) and return affected rows.
    /// </summary>
    /// <param name="sql">SQL command</param>
    /// <param name="parameters">Command parameters</param>
    /// <returns>Number of affected rows</returns>
    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        if (_config.EnableRetry)
        {
            return await RetryHelper.ExecuteAsync(
                async () => await _connection.ExecuteAsync(sql, parameters, commandTimeout: _config.CommandTimeout),
                maxAttempts: _config.MaxRetryCount
            );
        }

        return await _connection.ExecuteAsync(sql, parameters, commandTimeout: _config.CommandTimeout);
    }

    /// <summary>
    /// Get active users from the database.
    /// </summary>
    /// <returns>List of active users</returns>
    public async Task<List<User>> GetActiveUsersAsync()
    {
        var result = await QueryAsync<User>("SELECT * FROM Users WHERE Active = @Active", new { Active = true });
        return result.Data;
    }

    /// <summary>
    /// Get a user by ID.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User object or null</returns>
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await QuerySingleAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = userId });
    }

    /// <summary>
    /// Get a user by username.
    /// </summary>
    /// <param name="username">Username</param>
    /// <returns>User object or null</returns>
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await QuerySingleAsync<User>("SELECT * FROM Users WHERE Username = @Username", new { Username = username });
    }

    /// <summary>
    /// Create a test user in the database.
    /// </summary>
    /// <param name="user">User to create</param>
    /// <returns>Number of affected rows</returns>
    public async Task<int> CreateUserAsync(User user)
    {
        const string sql = @"
            INSERT INTO Users (Username, Email, Password, FirstName, LastName, Active, CreatedAt, Role)
            VALUES (@Username, @Email, @Password, @FirstName, @LastName, @Active, @CreatedAt, @Role)";

        return await ExecuteAsync(sql, new
        {
            user.Username,
            user.Email,
            user.Password,
            user.FirstName,
            user.LastName,
            user.Active,
            CreatedAt = DateTime.Now,
            user.Role
        });
    }

    /// <summary>
    /// Delete a test user by ID.
    /// </summary>
    /// <param name="userId">User ID to delete</param>
    /// <returns>Number of affected rows</returns>
    public async Task<int> DeleteUserAsync(int userId)
    {
        return await ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { Id = userId });
    }

    private static Dictionary<string, object> ConvertToDict(object parameters)
    {
        return parameters.GetType()
            .GetProperties()
            .ToDictionary(
                prop => prop.Name,
                prop => prop.GetValue(parameters) ?? DBNull.Value
            );
    }

    public async Task CleanupAsync()
    {
        if (_connection.State == ConnectionState.Open)
        {
            await Task.Run(() => _connection.Close());
        }
    }
}
