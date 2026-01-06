using System.Text;
using Allure.Net.Commons;
using Microsoft.Playwright;
using QuantumTestSuite.Framework.Core.Config;

namespace QuantumTestSuite.Framework.Core.Reporting;

public static class AllureHelper
{
    private static AppSettings? _settings;

    public static void Initialize(AppSettings settings)
    {
        _settings = settings;
    }

    public static void AttachJson(string name, string json)
    {
        Try(() => AllureApi.AddAttachment(name, "application/json", Encoding.UTF8.GetBytes(json), ".json"));
    }

    public static void AttachText(string name, string content)
    {
        Try(() => AllureApi.AddAttachment(name, "text/plain", Encoding.UTF8.GetBytes(content), ".txt"));
    }

    public static async Task AttachScreenshotAsync(string name, IPage page)
    {
        try
        {
            var bytes = await page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
            Try(() => AllureApi.AddAttachment(name, "image/png", bytes, ".png"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to capture screenshot: {ex.Message}");
        }
    }

    public static async Task CaptureScreenshotIfConfiguredAsync(IPage? page, string stepName, ScreenshotTiming timing)
    {
        if (page == null || _settings == null)
            return;

        var options = _settings.Playwright.ScreenshotOptions;
        var shouldCapture = timing switch
        {
            ScreenshotTiming.BeforeStep => options.BeforeStep,
            ScreenshotTiming.AfterStep => options.AfterStep,
            ScreenshotTiming.OnFailure => options.OnFailure,
            _ => false
        };

        if (shouldCapture)
        {
            var timingLabel = timing switch
            {
                ScreenshotTiming.BeforeStep => "before",
                ScreenshotTiming.AfterStep => "after",
                ScreenshotTiming.OnFailure => "failure",
                _ => "unknown"
            };

            // Format: screenshot-{stepName}-{timing}
            var screenshotName = $"screenshot-{stepName}-{timingLabel}";
            await AttachScreenshotAsync(screenshotName, page);
        }
    }

    public static void AttachRequestDetails(string method, string url, string? headers = null, string? body = null)
    {
        var details = new StringBuilder();
        details.AppendLine($"Method: {method}");
        details.AppendLine($"URL: {url}");
        
        if (!string.IsNullOrEmpty(headers))
        {
            details.AppendLine("\nHeaders:");
            details.AppendLine(headers);
        }
        
        if (!string.IsNullOrEmpty(body))
        {
            details.AppendLine("\nBody:");
            details.AppendLine(body);
        }

        AttachText("API Request", details.ToString());
        
        // Also attach as separate JSON if body is JSON
        if (!string.IsNullOrEmpty(body) && IsJson(body))
        {
            Try(() => AllureApi.AddAttachment("Request Body (JSON)", "application/json", 
                Encoding.UTF8.GetBytes(body), ".json"));
        }
    }

    public static void AttachResponseDetails(int statusCode, string? headers = null, string? body = null)
    {
        var details = new StringBuilder();
        details.AppendLine($"Status Code: {statusCode}");
        
        if (!string.IsNullOrEmpty(headers))
        {
            details.AppendLine("\nHeaders:");
            details.AppendLine(headers);
        }
        
        if (!string.IsNullOrEmpty(body))
        {
            details.AppendLine("\nBody:");
            details.AppendLine(body);
        }

        AttachText("API Response", details.ToString());
        
        // Also attach as separate JSON if body is JSON
        if (!string.IsNullOrEmpty(body) && IsJson(body))
        {
            Try(() => AllureApi.AddAttachment("Response Body (JSON)", "application/json", 
                Encoding.UTF8.GetBytes(body), ".json"));
        }
    }
    
    private static bool IsJson(string text)
    {
        text = text.Trim();
        return (text.StartsWith("{") && text.EndsWith("}")) || 
               (text.StartsWith("[") && text.EndsWith("]"));
    }

    public static async Task AttachVideoAsync(IPage page)
    {
        try
        {
            var videoPath = await page.Video!.PathAsync();
            if (File.Exists(videoPath))
            {
                var bytes = await File.ReadAllBytesAsync(videoPath);
                Try(() => AllureApi.AddAttachment("video-recording", "video/webm", bytes, ".webm"));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to attach video: {ex.Message}");
        }
    }

    /// <summary>
    /// Attaches video from a file path after context is closed (video fully saved)
    /// </summary>
    public static async Task AttachVideoFromPathAsync(string videoPath)
    {
        try
        {
            // Wait for video file to be fully written (up to 5 seconds)
            var maxWaitTime = TimeSpan.FromSeconds(5);
            var startTime = DateTime.Now;
            
            while (!File.Exists(videoPath) && DateTime.Now - startTime < maxWaitTime)
            {
                await Task.Delay(100);
            }

            if (File.Exists(videoPath))
            {
                // Wait a bit more to ensure file is fully written
                await Task.Delay(500);
                
                var bytes = await File.ReadAllBytesAsync(videoPath);
                var fileName = Path.GetFileName(videoPath);
                
                // Use AllureApi which is the correct public API
                Try(() => AllureApi.AddAttachment(
                    name: "Test Recording",
                    type: "video/webm",
                    content: bytes,
                    fileExtension: ".webm"));
                    
                Console.WriteLine($"✅ Video attached: {fileName} ({bytes.Length / 1024} KB)");
            }
            else
            {
                Console.WriteLine($"⚠️ Video file not found: {videoPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to attach video: {ex.Message}");
        }
    }

    private static void Try(Action action)
    {
        try
        {
            action();
        }
        catch
        {
            // Swallow attachment errors so tests are not blocked if Allure lifecycle is inactive
        }
    }
}

public enum ScreenshotTiming
{
    BeforeStep,
    AfterStep,
    OnFailure
}
