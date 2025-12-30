using Microsoft.Playwright;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.UI.Locators;

namespace QuantumTestSuite.UI.Pages;

public class SauceDemoLoginPage
{
    private readonly IPage _page;
    private readonly AppSettings _settings;

    public SauceDemoLoginPage(IPage page, AppSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    public async Task NavigateAsync()
    {
        var url = _settings.Playwright.BaseUrl.TrimEnd('/');
        await _page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
        await _page.WaitForSelectorAsync(SauceDemoLocators.UsernameInput);
    }

    public async Task LoginAsync(string username, string password)
    {
        await _page.FillAsync(SauceDemoLocators.UsernameInput, username);
        await _page.FillAsync(SauceDemoLocators.PasswordInput, password);
        await _page.ClickAsync(SauceDemoLocators.LoginButton);
    }

    public Task<bool> IsInventoryVisibleAsync() => _page.IsVisibleAsync(SauceDemoLocators.InventoryContainer);
    public Task<bool> HasErrorAsync() => _page.IsVisibleAsync(SauceDemoLocators.ErrorContainer);
}
