namespace QuantumTestSuite.Framework.UI.Locators;

/// <summary>
/// Centralized locators for Playwright Demo page
/// Based on: https://demo.playwright.dev/todomvc
/// </summary>
public static class PlaywrightDemoLocators
{
    // Page containers
    public const string MainContainer = ".todoapp";
    public const string Header = ".header";
    
    // Input elements
    public const string NewTodoInput = ".new-todo";
    public const string ToggleAll = ".toggle-all";
    
    // Todo list
    public const string TodoList = ".todo-list";
    public const string TodoItem = ".todo-list li";
    public const string TodoItemLabel = ".todo-list li label";
    public const string TodoCheckbox = ".todo-list li .toggle";
    public const string DeleteButton = ".todo-list li .destroy";
    
    // Filters
    public const string FiltersContainer = ".filters";
    public const string FilterAll = ".filters a[href='#/']";
    public const string FilterActive = ".filters a[href='#/active']";
    public const string FilterCompleted = ".filters a[href='#/completed']";
    
    // Info
    public const string TodoCount = ".todo-count";
    public const string ClearCompleted = ".clear-completed";
}
