using Microsoft.Playwright;
using QuantumTestSuite.UI.Locators;

namespace QuantumTestSuite.UI.Pages;

public class UltimateQaFormPage
{
    private readonly IPage _page;

    public UltimateQaFormPage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync("https://ultimateqa.com/filling-out-forms/");
    }

    public async Task SubmitFormAsync(string name, string email, string message)
    {
        await _page.WaitForSelectorAsync(UltimateQaLocators.Email);
        await _page.FillAsync(UltimateQaLocators.FirstName, name);
        await _page.FillAsync(UltimateQaLocators.Email, email);
        await _page.FillAsync(UltimateQaLocators.Message, message);
        await _page.ClickAsync(UltimateQaLocators.SubmitButton);
    }

    public Task<bool> IsSuccessVisibleAsync() => _page.IsVisibleAsync(UltimateQaLocators.Success);
}
