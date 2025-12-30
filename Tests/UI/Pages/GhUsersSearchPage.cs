using Microsoft.Playwright;
using QuantumTestSuite.UI.Locators;

namespace QuantumTestSuite.UI.Pages;

public class GhUsersSearchPage
{
    private readonly IPage _page;

    public GhUsersSearchPage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync(string baseUrl)
    {
        await _page.GotoAsync(baseUrl, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
        await _page.WaitForSelectorAsync(GhUsersSearchLocators.SearchInput);
    }

    public async Task SearchAsync(string username)
    {
        await _page.FillAsync(GhUsersSearchLocators.SearchInput, username);
        await _page.Keyboard.PressAsync("Enter");
        await _page.WaitForSelectorAsync(GhUsersSearchLocators.UserCard);
    }

    public async Task<bool> HasUserAsync(string username)
    {
        var cards = await _page.Locator(GhUsersSearchLocators.UserCard).AllAsync();
        foreach (var card in cards)
        {
            var login = await card.Locator(GhUsersSearchLocators.UserLogin).TextContentAsync();
            if (!string.IsNullOrWhiteSpace(login) && login.Trim().Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
