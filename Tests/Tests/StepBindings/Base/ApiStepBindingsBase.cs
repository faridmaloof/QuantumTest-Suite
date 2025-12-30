using QuantumTestSuite.API.Helpers;
using QuantumTestSuite.API.Models;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Core.Context;
using QuantumTestSuite.Core.Reporting;
using Reqnroll;
using System.Text.Json;

namespace QuantumTestSuite.Tests.StepBindings.Base;

/// <summary>
/// Base class for API step bindings with automatic request/response logging
/// </summary>
public abstract class ApiStepBindingsBase
{
    protected readonly ScenarioContext ScenarioContext;
    protected readonly AppSettings Settings;
    protected ApiTestContext Context => ScenarioContext.Get<ApiTestContext>();

    protected ApiStepBindingsBase(ScenarioContext scenarioContext, AppSettings settings)
    {
        ScenarioContext = scenarioContext;
        Settings = settings;
    }

    /// <summary>
    /// Executes an API call with automatic request/response logging to Allure
    /// </summary>
    protected async Task<ApiResponse<T>> ExecuteApiCallAsync<T>(
        string method,
        string url,
        Func<Task<ApiResponse<T>>> apiCall,
        object? requestBody = null,
        Dictionary<string, string>? headers = null)
    {
        // Log request details BEFORE the call
        LogRequestDetails(method, url, headers, requestBody);

        try
        {
            // Execute the API call
            var response = await apiCall();

            // Store last response in context
            Context.LastResponse = response.Body;

            // Log response details AFTER the call
            LogResponseDetails(response.StatusCode, response.Headers, response.Body);

            return response;
        }
        catch (Exception ex)
        {
            // Log exception
            AllureHelper.AttachText("API Error", $"Exception during {method} {url}:\n{ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Executes an API call with automatic logging (overload for simple response)
    /// </summary>
    protected async Task<ApiResponse<object>> ExecuteApiCallAsync(
        string method,
        string url,
        Func<Task<ApiResponse<object>>> apiCall,
        object? requestBody = null,
        Dictionary<string, string>? headers = null)
    {
        // Log request details BEFORE the call
        LogRequestDetails(method, url, headers, requestBody);

        try
        {
            // Execute the API call
            var response = await apiCall();

            // Store last response in context
            Context.LastResponse = response.Body;

            // Log response details AFTER the call
            LogResponseDetails(response.StatusCode, 
                response.Headers != null ? new Dictionary<string, string>(response.Headers) : null, 
                response.Body);

            return response;
        }
        catch (Exception ex)
        {
            // Log exception
            AllureHelper.AttachText("API Error", $"Exception during {method} {url}:\n{ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Logs request details to Allure
    /// </summary>
    private void LogRequestDetails(string method, string url, Dictionary<string, string>? headers, object? body)
    {
        var headersStr = headers != null 
            ? string.Join("\n", headers.Select(h => $"{h.Key}: {h.Value}"))
            : "Content-Type: application/json";

        var bodyStr = body != null
            ? JsonSerializer.Serialize(body, new JsonSerializerOptions { WriteIndented = true })
            : null;

        AllureHelper.AttachRequestDetails(method, url, headersStr, bodyStr);
    }

    /// <summary>
    /// Logs response details to Allure
    /// </summary>
    private void LogResponseDetails(int statusCode, IDictionary<string, string>? headers, string? body)
    {
        var headersStr = headers != null
            ? string.Join("\n", headers.Select(h => $"{h.Key}: {h.Value}"))
            : null;

        AllureHelper.AttachResponseDetails(statusCode, headersStr, body);
    }

    /// <summary>
    /// Helper to format headers for display
    /// </summary>
    protected string FormatHeaders(IDictionary<string, string>? headers)
    {
        if (headers == null || headers.Count == 0)
            return "No headers";

        return string.Join("\n", headers.Select(h => $"{h.Key}: {h.Value}"));
    }
}
