using Microsoft.Playwright;
using QuantumTestSuite.UI.Locators;

namespace QuantumTestSuite.UI.Pages;

public class SauceDemoLoginPage
{
    private readonly IPage _page;

    public SauceDemoLoginPage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync("/", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
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
