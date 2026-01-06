using System.Text;
using QuantumTestSuite.Framework.Core.Config;

namespace QuantumTestSuite.Framework.Core.Reporting;

/// <summary>
/// Writes environment information for Allure reports
/// </summary>
public static class AllureEnvironmentWriter
{
    public static void WriteEnvironmentInfo(string allureResultsPath)
    {
        var settings = ConfigManager.Settings;
        var envFilePath = Path.Combine(allureResultsPath, "environment.properties");
        
        var properties = new StringBuilder();
        properties.AppendLine($"Test.Environment={GetEnvironmentName()}");
        properties.AppendLine($"Framework=QuantumTestSuite");
        properties.AppendLine($"Framework.Version=1.0.0");
        properties.AppendLine($"DotNet.Version={Environment.Version}");
        properties.AppendLine($"OS={Environment.OSVersion}");
        properties.AppendLine($"Machine={Environment.MachineName}");
        properties.AppendLine($"User={Environment.UserName}");
        properties.AppendLine($"Browser={settings.Playwright.Browser}");
        properties.AppendLine($"Headless={settings.Playwright.Headless}");
        properties.AppendLine($"Video.Enabled={settings.Playwright.VideoEnabled}");
        properties.AppendLine($"Screenshot.OnFailure={settings.Playwright.ScreenshotOptions.OnFailure}");
        properties.AppendLine($"Execution.Date={DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        
        Directory.CreateDirectory(Path.GetDirectoryName(envFilePath)!);
        File.WriteAllText(envFilePath, properties.ToString());
    }
    
    private static string GetEnvironmentName()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                  ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                  ?? "Development";
        return env;
    }
}
