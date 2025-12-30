namespace QuantumTestSuite.Core.Config;

public class AppSettings
{
    public string Env { get; set; } = "local";
    public PlaywrightSettings Playwright { get; set; } = new();
    public ApiSettings Apis { get; set; } = new();
    public UserSettings Users { get; set; } = new();
    public AllureSettings Allure { get; set; } = new();

}

public class AllureSettings
{
    public string Directory { get; set; } = "Reports/AllureResults";
    public int CleanupCycle { get; set; } = 1;
}

public class PlaywrightSettings
{
    public bool Headless { get; set; } = true;
    public string Browser { get; set; } = "chromium";
    public string BaseUrl { get; set; } = string.Empty;
    public int SlowMo { get; set; } = 0;
    public bool VideoEnabled { get; set; } = false;
    public ScreenshotOptions ScreenshotOptions { get; set; } = new();
}

public class ScreenshotOptions
{
    public bool BeforeStep { get; set; } = false;
    public bool AfterStep { get; set; } = false;
    public bool OnFailure { get; set; } = true;
}

public class ApiSettings
{
    public string HttpBin { get; set; } = string.Empty;
    public string RestfulBooker { get; set; } = string.Empty;
    public string GitHub { get; set; } = string.Empty;
    public string GhUsersSearchUi { get; set; } = string.Empty;
}

public class UserSettings
{
    public SauceDemoUser SauceDemo { get; set; } = new();
}

public class SauceDemoUser
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
