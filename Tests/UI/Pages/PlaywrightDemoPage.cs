using Microsoft.Playwright;
using QuantumTestSuite.UI.Locators;
using QuantumTestSuite.Core.Config;

namespace QuantumTestSuite.UI.Pages;

/// <summary>
/// Page Object for Playwright TodoMVC Demo
/// URL: https://demo.playwright.dev/todomvc
/// </summary>
public class PlaywrightDemoPage
{
    private readonly IPage _page;
    private readonly AppSettings? _settings;

    public PlaywrightDemoPage(IPage page, AppSettings? settings = null)
    {
        _page = page;
        _settings = settings;
    }

    public async Task NavigateAsync()
    {
        var url = "https://demo.playwright.dev/todomvc";
        await _page.GotoAsync(url, new PageGotoOptions 
        { 
            WaitUntil = WaitUntilState.NetworkIdle 
        });
        await _page.WaitForSelectorAsync(PlaywrightDemoLocators.MainContainer);
    }

    public async Task<bool> IsVisibleAsync()
    {
        return await _page.IsVisibleAsync(PlaywrightDemoLocators.MainContainer);
    }

    public async Task AddTodoAsync(string todoText)
    {
        await _page.FillAsync(PlaywrightDemoLocators.NewTodoInput, todoText);
        await _page.PressAsync(PlaywrightDemoLocators.NewTodoInput, "Enter");
        
        // Wait for the item to appear
        await Task.Delay(300);
    }

    public async Task<int> GetTodoCountAsync()
    {
        return await _page.Locator(PlaywrightDemoLocators.TodoItem).CountAsync();
    }

    public async Task<List<string>> GetTodoTextsAsync()
    {
        var locator = _page.Locator(PlaywrightDemoLocators.TodoItemLabel);
        var count = await locator.CountAsync();
        var texts = new List<string>();

        for (int i = 0; i < count; i++)
        {
            var text = await locator.Nth(i).InnerTextAsync();
            texts.Add(text);
        }

        return texts;
    }

    public async Task ToggleTodoAsync(int index)
    {
        await _page.Locator(PlaywrightDemoLocators.TodoCheckbox).Nth(index).ClickAsync();
    }

    public async Task DeleteTodoAsync(int index)
    {
        var todoItem = _page.Locator(PlaywrightDemoLocators.TodoItem).Nth(index);
        await todoItem.HoverAsync();
        await todoItem.Locator(PlaywrightDemoLocators.DeleteButton).ClickAsync();
    }

    public async Task<bool> IsTodoCompletedAsync(int index)
    {
        var todoItem = _page.Locator(PlaywrightDemoLocators.TodoItem).Nth(index);
        var classAttr = await todoItem.GetAttributeAsync("class");
        return classAttr?.Contains("completed") ?? false;
    }

    public async Task<string> GetRemainingCountTextAsync()
    {
        return await _page.InnerTextAsync(PlaywrightDemoLocators.TodoCount);
    }

    public async Task ClearCompletedAsync()
    {
        if (await _page.IsVisibleAsync(PlaywrightDemoLocators.ClearCompleted))
        {
            await _page.ClickAsync(PlaywrightDemoLocators.ClearCompleted);
        }
    }

    public async Task FilterByAsync(string filter)
    {
        var selector = filter.ToLower() switch
        {
            "all" => PlaywrightDemoLocators.FilterAll,
            "active" => PlaywrightDemoLocators.FilterActive,
            "completed" => PlaywrightDemoLocators.FilterCompleted,
            _ => PlaywrightDemoLocators.FilterAll
        };

        await _page.ClickAsync(selector);
    }
}
