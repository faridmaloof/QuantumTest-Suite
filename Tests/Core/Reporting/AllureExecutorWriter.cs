using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuantumTestSuite.Core.Reporting;

/// <summary>
/// Writes executor information for Allure reports (CI/CD integration)
/// </summary>
public static class AllureExecutorWriter
{
    public static void WriteExecutorInfo(string allureResultsPath)
    {
        var executor = GetExecutorInfo();
        var executorFilePath = Path.Combine(allureResultsPath, "executor.json");
        
        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        
        var json = JsonSerializer.Serialize(executor, options);
        
        Directory.CreateDirectory(Path.GetDirectoryName(executorFilePath)!);
        File.WriteAllText(executorFilePath, json);
    }
    
    private static ExecutorInfo GetExecutorInfo()
    {
        // GitHub Actions
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS")))
        {
            return new ExecutorInfo
            {
                Name = "GitHub Actions",
                Type = "github",
                Url = GetGitHubActionUrl(),
                BuildOrder = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER"),
                BuildName = $"#{Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER")}",
                BuildUrl = GetGitHubActionUrl(),
                ReportUrl = GetGitHubActionUrl(),
                ReportName = $"Run #{Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER")}"
            };
        }
        
        // Azure DevOps
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TF_BUILD")))
        {
            return new ExecutorInfo
            {
                Name = "Azure DevOps",
                Type = "azure",
                Url = Environment.GetEnvironmentVariable("SYSTEM_TEAMFOUNDATIONCOLLECTIONURI"),
                BuildOrder = Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER"),
                BuildName = Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER"),
                BuildUrl = GetAzureDevOpsBuildUrl(),
                ReportName = $"Build {Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER")}"
            };
        }
        
        // Jenkins
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JENKINS_HOME")))
        {
            return new ExecutorInfo
            {
                Name = "Jenkins",
                Type = "jenkins",
                Url = Environment.GetEnvironmentVariable("JENKINS_URL"),
                BuildOrder = Environment.GetEnvironmentVariable("BUILD_NUMBER"),
                BuildName = $"#{Environment.GetEnvironmentVariable("BUILD_NUMBER")}",
                BuildUrl = Environment.GetEnvironmentVariable("BUILD_URL"),
                ReportName = $"Build #{Environment.GetEnvironmentVariable("BUILD_NUMBER")}"
            };
        }
        
        // GitLab CI
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITLAB_CI")))
        {
            return new ExecutorInfo
            {
                Name = "GitLab CI",
                Type = "gitlab",
                Url = Environment.GetEnvironmentVariable("CI_PROJECT_URL"),
                BuildOrder = Environment.GetEnvironmentVariable("CI_PIPELINE_ID"),
                BuildName = $"Pipeline #{Environment.GetEnvironmentVariable("CI_PIPELINE_ID")}",
                BuildUrl = $"{Environment.GetEnvironmentVariable("CI_PROJECT_URL")}/-/pipelines/{Environment.GetEnvironmentVariable("CI_PIPELINE_ID")}",
                ReportName = $"Pipeline {Environment.GetEnvironmentVariable("CI_PIPELINE_ID")}"
            };
        }
        
        // Local execution
        return new ExecutorInfo
        {
            Name = "Local",
            Type = "local",
            BuildName = $"Local-{DateTime.Now:yyyyMMdd-HHmmss}",
            ReportName = $"Local Execution - {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
        };
    }
    
    private static string? GetGitHubActionUrl()
    {
        var serverUrl = Environment.GetEnvironmentVariable("GITHUB_SERVER_URL");
        var repository = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
        var runId = Environment.GetEnvironmentVariable("GITHUB_RUN_ID");
        
        if (string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(repository) || string.IsNullOrEmpty(runId))
            return null;
            
        return $"{serverUrl}/{repository}/actions/runs/{runId}";
    }
    
    private static string? GetAzureDevOpsBuildUrl()
    {
        var collectionUri = Environment.GetEnvironmentVariable("SYSTEM_TEAMFOUNDATIONCOLLECTIONURI");
        var project = Environment.GetEnvironmentVariable("SYSTEM_TEAMPROJECT");
        var buildId = Environment.GetEnvironmentVariable("BUILD_BUILDID");
        
        if (string.IsNullOrEmpty(collectionUri) || string.IsNullOrEmpty(project) || string.IsNullOrEmpty(buildId))
            return null;
            
        return $"{collectionUri}{project}/_build/results?buildId={buildId}";
    }
}

public class ExecutorInfo
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    
    [JsonPropertyName("buildOrder")]
    public string? BuildOrder { get; set; }
    
    [JsonPropertyName("buildName")]
    public string? BuildName { get; set; }
    
    [JsonPropertyName("buildUrl")]
    public string? BuildUrl { get; set; }
    
    [JsonPropertyName("reportUrl")]
    public string? ReportUrl { get; set; }
    
    [JsonPropertyName("reportName")]
    public string? ReportName { get; set; }
}
