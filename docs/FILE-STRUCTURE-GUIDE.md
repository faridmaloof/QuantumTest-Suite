# QuantumTest-Suite - File Structure Guide for AI Code Generation

> **Version**: 2.0.0  
> **Last Updated**: January 2025  
> **Purpose**: Definitive architecture guide for AI agents generating test automation code

---

## 📋 Table of Contents

1. [Framework Overview](#framework-overview)
2. [Directory Structure](#directory-structure)
3. [File Naming Conventions](#file-naming-conventions)
4. [Screenplay Pattern Architecture](#screenplay-pattern-architecture)
5. [Code Templates](#code-templates)
6. [Questions Pattern Implementation](#questions-pattern-implementation)
7. [Validation Rules](#validation-rules)
8. [Examples](#examples)
9. [Anti-Patterns](#anti-patterns)

---

## 🎯 Framework Overview

**QuantumTest-Suite** is an enterprise-grade .NET 8 test automation framework combining:

- **BDD**: Reqnroll (SpecFlow successor) for Gherkin scenarios
- **UI Automation**: Playwright for cross-browser testing
- **API Testing**: Native HTTP clients + Playwright API contexts
- **Reporting**: Allure for comprehensive test reports
- **Pattern**: Screenplay Pattern with Abilities, Tasks, and Questions

### Key Principles

1. **Separation of Concerns**: Each component has a single responsibility
2. **Maintainability**: Centralized locators, reusable tasks
3. **Scalability**: Support for multiple environments and configurations
4. **Type Safety**: Strong typing throughout the codebase
5. **Async/Await**: All I/O operations are asynchronous

---

## 📂 Directory Structure

```
QuantumTest-Suite/
├── Tests/
│   ├── Features/                          # Gherkin feature files (BDD scenarios)
│   │   ├── UiFeatures/                    # UI test scenarios
│   │   │   └── *.feature                  # snake_case naming
│   │   ├── ApiFeatures/                   # API test scenarios
│   │   │   └── *.feature                  # snake_case naming
│   │   └── UnitFeatures/                  # Unit test scenarios
│   │       └── *.feature                  # snake_case naming
│   │
│   ├── Tests/                             # Test execution layer
│   │   └── StepBindings/                  # Step definitions (Reqnroll bindings)
│   │       ├── Base/                      # Base classes for step bindings
│   │       │   ├── UiStepBindingsBase.cs
│   │       │   └── ApiStepBindingsBase.cs
│   │       ├── Ui/                        # UI step bindings
│   │       │   └── *StepBindings.cs       # PascalCase + StepBindings suffix
│   │       ├── Api/                       # API step bindings
│   │       │   └── *StepBindings.cs       # PascalCase + StepBindings suffix
│   │       └── Unit/                      # Unit test step bindings
│   │           └── *StepBindings.cs       # PascalCase + StepBindings suffix
│   │
│   ├── UI/                                # UI automation components
│   │   ├── Pages/                         # Page Object Model (POM)
│   │   │   └── *Page.cs                   # PascalCase + Page suffix
│   │   ├── Locators/                      # Centralized element locators
│   │   │   └── *Locators.cs               # PascalCase + Locators suffix (static class)
│   │   ├── Screenplay/                    # Screenplay Pattern implementation
│   │   │   ├── Actors/                    # Actor class (who performs actions)
│   │   │   │   └── Actor.cs
│   │   │   ├── Abilities/                 # Abilities (what actors can do)
│   │   │   │   ├── IAbility.cs            # Base interface
│   │   │   │   ├── RememberData.cs        # In-memory data storage
│   │   │   │   ├── AccessDatabase.cs      # Database operations
│   │   │   │   ├── CallApiEndpoint.cs     # API interactions
│   │   │   │   └── ReadConfiguration.cs   # Configuration access
│   │   │   ├── Tasks/                     # Tasks (high-level actions)
│   │   │   │   └── *.cs                   # VerbNoun pattern (e.g., LoginToApp, FillForm)
│   │   │   └── Questions/                 # ✨ Questions (data retrieval & assertions)
│   │   │       ├── IQuestion.cs           # Generic question interface
│   │   │       ├── TheText.cs             # Get text from elements
│   │   │       ├── TheVisibility.cs       # Check element visibility
│   │   │       ├── TheValue.cs            # Get input values
│   │   │       ├── TheCount.cs            # Count elements
│   │   │       ├── TheCurrentUrl.cs       # Get current page URL
│   │   │       └── TheTitle.cs            # Get page title
│   │   └── Drivers/                       # Browser/driver management
│   │       └── PlaywrightDriver.cs
│   │
│   ├── API/                               # API testing components
│   │   ├── Clients/                       # HTTP clients for different APIs
│   │   │   └── *Client.cs                 # PascalCase + Client suffix
│   │   ├── Models/                        # Response/request models
│   │   │   └── *.cs                       # PascalCase, matches API structure
│   │   ├── Questions/                     # ✨ API-specific questions
│   │   │   ├── IApiQuestion.cs            # API question interface
│   │   │   ├── TheResponseStatus.cs       # Get HTTP status code
│   │   │   ├── TheResponseBody.cs         # Get & deserialize response body
│   │   │   └── The*.cs                    # Custom API questions
│   │   └── Helpers/                       # API utilities
│   │       └── ApiHelpers.cs
│   │
│   ├── Core/                              # Framework core components
│   │   ├── Config/                        # Configuration management
│   │   │   ├── AppSettings.cs             # Settings model
│   │   │   └── ConfigManager.cs           # Configuration loader
│   │   ├── Hooks/                         # Reqnroll hooks (Before/After)
│   │   │   ├── TestHooks.cs               # Global setup/teardown
│   │   │   └── UiTestHooks.cs             # UI-specific hooks
│   │   ├── Reporting/                     # Allure reporting helpers
│   │   │   └── AllureHelper.cs
│   │   └── DependencyInjection/           # IoC container configuration
│   │       └── DependencyInjectionConfig.cs
│   │
│   └── appsettings.json                   # Environment configuration
│
├── docs/                                  # Documentation
│   ├── ARCHITECTURE.md                    # Framework architecture overview
│   ├── ABILITIES-GUIDE.md                 # Screenplay Abilities guide
│   ├── QUESTIONS-GUIDE.md                 # ✨ Questions Pattern guide
│   ├── ALLURE-QUICKSTART.md               # Allure reporting setup
│   └── FILE-STRUCTURE-GUIDE.md            # This document
│
└── QuantumTestSuite.Tests.csproj          # Project file

```

### ✨ New in Version 2.0: Questions Pattern

The framework now includes a comprehensive **Questions** layer for data retrieval and assertions:

- **UI Questions**: `Tests/UI/Screenplay/Questions/` - For UI element queries
- **API Questions**: `Tests/API/Questions/` - For API response assertions
- Fluent API design for readable assertions
- Integrates seamlessly with the Screenplay Pattern

---

## 📝 File Naming Conventions

### Feature Files (Gherkin)

**Location**: `Tests/Features/{UiFeatures|ApiFeatures|UnitFeatures}/`  
**Naming**: `snake_case.feature`  
**Examples**:
- ✅ `playwright_demo.feature`
- ✅ `pokemon_api.feature`
- ✅ `screenplay_pattern_tests.feature`
- ❌ `PlaywrightDemo.feature` (wrong case)
- ❌ `playwright-demo.feature` (wrong delimiter)

### Step Bindings

**Location**: `Tests/Tests/StepBindings/{Ui|Api|Unit}/`  
**Naming**: `PascalCase` + `StepBindings.cs` suffix  
**Examples**:
- ✅ `PlaywrightDemoStepBindings.cs`
- ✅ `PokemonApiStepBindings.cs`
- ✅ `ScreenplayPatternStepBindings.cs`
- ❌ `PlaywrightDemo.cs` (missing suffix)
- ❌ `playwright_demo_step_bindings.cs` (wrong case)

### Page Objects

**Location**: `Tests/UI/Pages/`  
**Naming**: `PascalCase` + `Page.cs` suffix  
**Examples**:
- ✅ `PlaywrightDemoPage.cs`
- ✅ `LoginPage.cs`
- ❌ `PlaywrightDemo.cs` (missing suffix)

### Locators

**Location**: `Tests/UI/Locators/`  
**Naming**: `PascalCase` + `Locators.cs` suffix  
**Must be**: Static class with const string fields  
**Examples**:
- ✅ `PlaywrightDemoLocators.cs`
- ✅ `LoginLocators.cs`

### Tasks

**Location**: `Tests/UI/Screenplay/Tasks/`  
**Naming**: `VerbNoun` pattern in `PascalCase`  
**Examples**:
- ✅ `AddTodoItem.cs`
- ✅ `NavigateToPlaywrightDemo.cs`
- ✅ `LoginToApplication.cs`
- ❌ `TodoItem.cs` (missing verb)

### Questions (NEW) ✨

**Location**: 
- UI: `Tests/UI/Screenplay/Questions/`
- API: `Tests/API/Questions/`

**Naming**: `The{Property}.cs` pattern  
**Examples**:
- ✅ `TheText.cs` - Retrieves text content
- ✅ `TheVisibility.cs` - Checks visibility
- ✅ `TheResponseStatus.cs` - Gets HTTP status
- ✅ `TheTodoItems.cs` - Custom question

### API Clients

**Location**: `Tests/API/Clients/`  
**Naming**: `PascalCase` + `Client.cs` suffix  
**Examples**:
- ✅ `PokeApiClient.cs`
- ✅ `GitHubClient.cs`

---

## 🎭 Screenplay Pattern Architecture

The Screenplay Pattern organizes test code into clear layers:

### 1. Actors (Who)

**Location**: `Tests/UI/Screenplay/Actors/Actor.cs`  
**Purpose**: Represents a user/system performing actions

```csharp
var actor = new Actor("TestUser", page)
    .WhoCan(new RememberData())
    .WhoCan(new CallApiEndpoint(apiContext, settings));
```

**Key Methods**:
- `AttemptsTo(params ITask[] tasks)` - Execute tasks
- `Asks<T>(IQuestion<T> question)` - ✨ Ask questions (NEW)
- `Using<TAbility>()` - Access abilities

### 2. Abilities (What They Can Do)

**Location**: `Tests/UI/Screenplay/Abilities/`  
**Purpose**: Enable actors to interact with different layers

**Available Abilities**:
- `RememberData` - Store/retrieve test data
- `CallApiEndpoint` - Make API calls
- `AccessDatabase` - Database operations
- `ReadConfiguration` - Access app settings

### 3. Tasks (How They Do It)

**Location**: `Tests/UI/Screenplay/Tasks/`  
**Purpose**: High-level business actions

**Structure**:
```csharp
public class AddTodoItem : ITask
{
    private readonly string _todoText;

    public static AddTodoItem With(string text) => new(text);

    public async Task ExecuteAsync(Actor actor)
    {
        // Implementation using Pages
    }
}
```

### 4. Questions (What They See) ✨ NEW

**Location**: 
- `Tests/UI/Screenplay/Questions/` (UI)
- `Tests/API/Questions/` (API)

**Purpose**: Retrieve information for assertions

**UI Question Example**:
```csharp
public class TheText : IQuestion<string>
{
    public static TheText Of(string selector) => new(selector);

    public async Task<string> AnsweredBy(Actor actor)
    {
        var element = await actor.Page.WaitForSelectorAsync(_selector);
        return await element.InnerTextAsync();
    }
}
```

**API Question Example**:
```csharp
public class TheResponseStatus : IApiQuestion<HttpStatusCode>
{
    public static TheResponseStatus Code => _instance;

    public async Task<HttpStatusCode> AnsweredBy(Actor actor)
    {
        var response = actor.Using<CallApiEndpoint>().GetLastResponse();
        return response.StatusCode;
    }
}
```

**Usage in Step Bindings**:
```csharp
// UI Question
var text = await Actor!.Asks(TheText.Of(".message"));
Assert.That(text, Is.EqualTo("Expected text"));

// Check visibility
var isVisible = await Actor!.Asks(TheVisibility.Of("#button"));
Assert.That(isVisible, Is.True);

// Count elements
var count = await Actor!.Asks(TheCount.Of(".list-item"));
Assert.That(count, Is.EqualTo(5));

// API Question
var status = await Actor!.Asks(TheResponseStatus.Code);
Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
```

---

## 📦 Code Templates

### 1. Feature File Template

**Location**: `Tests/Features/{UiFeatures|ApiFeatures|UnitFeatures}/*.feature`

```gherkin
@allure.parentSuite:UI-Tests
@allure.suite:SuiteName
@allure.feature:FeatureName
@allure.owner:QA-Team
Feature: Feature Title
  As a [role]
  I want [feature]
  So that [benefit]

  @ui @smoke
  @allure.story:StoryName
  @allure.severity:critical
  Scenario: Scenario Title
    Given precondition
    When action
    Then expected result
```

### 2. Step Bindings Template

**Location**: `Tests/Tests/StepBindings/{Ui|Api|Unit}/*StepBindings.cs`

```csharp
using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Tests.StepBindings.Base;
using QuantumTestSuite.UI.Screenplay.Tasks;
using QuantumTestSuite.UI.Screenplay.Questions;
using Reqnroll;

namespace QuantumTestSuite.Tests.StepBindings.Ui;

[Binding]
[AllureParentSuite("UI Tests")]
[AllureSuite("Suite Name")]
[AllureFeature("Feature Name")]
public class FeatureNameStepBindings : UiStepBindingsBase
{
    public FeatureNameStepBindings(
        ScenarioContext scenarioContext,
        AppSettings settings)
        : base(scenarioContext, settings)
    {
    }

    [Given(@"step pattern")]
    public async Task GivenStep()
    {
        await ExecuteGivenAsync(async () =>
        {
            await EnsureActorAsync("User1");
            // Implementation
        });
    }

    [When(@"step pattern")]
    public async Task WhenStep()
    {
        await ExecuteWhenAsync(async () =>
        {
            await Actor!.AttemptsTo(SomeTask.WithParams());
        });
    }

    [Then(@"step pattern")]
    public async Task ThenStep()
    {
        await ExecuteThenAsync(async () =>
        {
            // Use Questions for assertions
            var result = await Actor!.Asks(TheText.Of(".selector"));
            Assert.That(result, Is.EqualTo("expected"));
        });
    }
}
```

### 3. Page Object Template

**Location**: `Tests/UI/Pages/*Page.cs`

```csharp
using Microsoft.Playwright;
using QuantumTestSuite.UI.Locators;
using QuantumTestSuite.Core.Config;

namespace QuantumTestSuite.UI.Pages;

/// <summary>
/// Page Object for [PageName] page
/// URL: [page URL]
/// </summary>
public class PageNamePage
{
    private readonly IPage _page;
    private readonly AppSettings? _settings;

    public PageNamePage(IPage page, AppSettings? settings = null)
    {
        _page = page;
        _settings = settings;
    }

    public async Task NavigateAsync()
    {
        var url = _settings?.Playwright?.BaseUrl ?? "https://example.com";
        await _page.GotoAsync(url, new PageGotoOptions 
        { 
            WaitUntil = WaitUntilState.NetworkIdle 
        });
        await _page.WaitForSelectorAsync(PageNameLocators.MainContainer);
    }

    public async Task<bool> IsVisibleAsync()
    {
        return await _page.IsVisibleAsync(PageNameLocators.MainContainer);
    }

    // Add specific page methods here
    public async Task ClickButtonAsync()
    {
        await _page.ClickAsync(PageNameLocators.SubmitButton);
    }
}
```

### 4. Locators Template

**Location**: `Tests/UI/Locators/*Locators.cs`

```csharp
namespace QuantumTestSuite.UI.Locators;

/// <summary>
/// Centralized locators for [PageName] page
/// </summary>
public static class PageNameLocators
{
    // Main containers
    public const string MainContainer = ".main-container";
    
    // Input elements
    public const string UsernameInput = "input[name='username']";
    public const string PasswordInput = "input[type='password']";
    
    // Buttons
    public const string SubmitButton = "button[type='submit']";
    public const string CancelButton = ".btn-cancel";
    
    // Messages/alerts
    public const string ErrorMessage = ".error-message";
    public const string SuccessMessage = ".success";
}
```

### 5. Task Template

**Location**: `Tests/UI/Screenplay/Tasks/*.cs`

```csharp
using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Actors;
using QuantumTestSuite.Core.Config;

namespace QuantumTestSuite.UI.Screenplay.Tasks;

/// <summary>
/// Task to [action description]
/// </summary>
public class TaskName : ITask
{
    private readonly string _param;
    private readonly AppSettings? _settings;

    private TaskName(string param, AppSettings? settings = null)
    {
        _param = param;
        _settings = settings;
    }

    public static TaskName With(string param) => new(param);

    public async Task ExecuteAsync(Actor actor)
    {
        var page = new SomePage(actor.Page, _settings);
        await page.NavigateAsync();
        await page.DoSomethingAsync(_param);
    }
}
```

### 6. UI Question Template ✨ NEW

**Location**: `Tests/UI/Screenplay/Questions/*.cs`

```csharp
namespace QuantumTestSuite.UI.Screenplay.Questions;

/// <summary>
/// Question to retrieve [description]
/// </summary>
public class TheSomething : IQuestion<string>
{
    private readonly string _selector;

    private TheSomething(string selector)
    {
        _selector = selector;
    }

    public static TheSomething Of(string selector) => new(selector);

    public async Task<string> AnsweredBy(Actors.Actor actor)
    {
        var element = await actor.Page.WaitForSelectorAsync(_selector);
        // Extract and return the information
        return await element.InnerTextAsync();
    }
}
```

### 7. API Client Template

**Location**: `Tests/API/Clients/*Client.cs`

```csharp
using System.Net.Http;
using System.Text.Json;
using QuantumTestSuite.API.Models;

namespace QuantumTestSuite.API.Clients;

/// <summary>
/// Client for [API Name]
/// Base URL: [API base URL]
/// </summary>
public class ApiNameClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiNameClient(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient 
        { 
            BaseAddress = new Uri("https://api.example.com/") 
        };
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<(Model? Data, HttpResponseMessage Response)> GetResourceAsync(int id)
    {
        var response = await _httpClient.GetAsync($"resource/{id}");
        
        if (!response.IsSuccessStatusCode)
            return (null, response);

        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<Model>(content, _jsonOptions);
        
        return (data, response);
    }
}
```

### 8. API Question Template ✨ NEW

**Location**: `Tests/API/Questions/*.cs`

```csharp
using QuantumTestSuite.UI.Screenplay.Abilities;

namespace QuantumTestSuite.API.Questions;

/// <summary>
/// Question to retrieve [description] from API response
/// </summary>
public class TheSomething : IApiQuestion<SomeType>
{
    private static readonly TheSomething _instance = new();

    private TheSomething() { }

    public static TheSomething Value => _instance;

    public async Task<SomeType> AnsweredBy(UI.Screenplay.Actors.Actor actor)
    {
        var apiAbility = actor.Using<CallApiEndpoint>();
        var response = apiAbility.GetLastResponse();
        
        if (response == null)
            throw new InvalidOperationException("No API response available");

        // Extract and return the data
        var content = await response.Content.ReadAsStringAsync();
        return System.Text.Json.JsonSerializer.Deserialize<SomeType>(content);
    }
}
```

---

## ✅ Validation Rules

### Path Validation

**Correct Paths**:
- ✅ `Tests/Features/UiFeatures/`
- ✅ `Tests/Tests/StepBindings/Ui/`
- ✅ `Tests/UI/Pages/`
- ✅ `Tests/UI/Locators/`
- ✅ `Tests/UI/Screenplay/Tasks/`
- ✅ `Tests/UI/Screenplay/Questions/` ✨ NEW
- ✅ `Tests/API/Clients/`
- ✅ `Tests/API/Questions/` ✨ NEW

**Deprecated/Wrong Paths**:
- ❌ `Features/` (root level - deprecated)
- ❌ `Core/PageObjects/` (old POM location)
- ❌ `Tests/UI/StepBindings/` (wrong location)

### Naming Validation

- Feature files: Must be `snake_case.feature`
- Step bindings: Must end with `StepBindings.cs`
- Page objects: Must end with `Page.cs`
- Locators: Must end with `Locators.cs` and be static
- Tasks: Must follow `VerbNoun` pattern
- Questions: Must start with `The` prefix ✨

### Dependency Validation

**Import Order**:
1. System namespaces
2. Microsoft namespaces
3. Third-party namespaces (Allure, NUnit, Reqnroll)
4. QuantumTestSuite namespaces

**Required Using Statements** (Step Bindings):
```csharp
using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Tests.StepBindings.Base;
using QuantumTestSuite.UI.Screenplay.Tasks;
using QuantumTestSuite.UI.Screenplay.Questions;  // ✨ NEW
using Reqnroll;
```

---

## 📚 Examples

### Example 1: Complete UI Feature (Playwright TodoMVC Demo)

**Feature**: `Tests/Features/UiFeatures/playwright_demo.feature`
```gherkin
@allure.parentSuite:UI-Tests
@allure.suite:Playwright-Demo
@allure.feature:InteractiveElements
Feature: Playwright Interactive Testing Demo
  
  @ui @smoke @demo
  Scenario: Add and verify items in a dynamic list
    Given the user navigates to the Playwright demo page
    When the user adds "Item 1" to the list
    And the user adds "Item 2" to the list
    Then the list should contain 2 items
    And the list should include "Item 1"
```

**Step Bindings**: `Tests/Tests/StepBindings/Ui/PlaywrightDemoStepBindings.cs`
```csharp
[When(@"the user adds ""(.*)"" to the list")]
public async Task WhenTheUserAddsItemToTheList(string itemText)
{
    await ExecuteWhenAsync(async () =>
    {
        await Actor!.AttemptsTo(AddTodoItem.With(itemText));
    });
}

[Then(@"the list should contain (.*) items")]
public async Task ThenTheListShouldContainItems(int expectedCount)
{
    await ExecuteThenAsync(async () =>
    {
        // ✨ Using Questions Pattern
        var actualCount = await Actor!.Asks(
            TheCount.Of(PlaywrightDemoLocators.TodoItem)
        );
        
        Assert.That(actualCount, Is.EqualTo(expectedCount));
    });
}
```

**Complete Files**:
- Page: `Tests/UI/Pages/PlaywrightDemoPage.cs`
- Locators: `Tests/UI/Locators/PlaywrightDemoLocators.cs`
- Task: `Tests/UI/Screenplay/Tasks/AddTodoItem.cs`
- Questions: `Tests/UI/Screenplay/Questions/TheTodoItems.cs` ✨

### Example 2: Complete API Feature (PokeAPI)

**Feature**: `Tests/Features/ApiFeatures/pokemon_api.feature`
```gherkin
@allure.parentSuite:API-Tests
@allure.suite:Pokemon-API
Feature: Pokemon API Testing
  
  @api @smoke
  Scenario: Retrieve Pokemon by name
    When I request pokemon "pikachu"
    Then the response status should be 200
    And the pokemon should have "electric" type
```

**Step Bindings**: `Tests/Tests/StepBindings/Api/PokemonApiStepBindings.cs`
```csharp
[When(@"I request pokemon ""(.*)""")]
public async Task WhenIRequestPokemon(string pokemonName)
{
    var result = await _pokeApiClient.GetPokemonByNameAsync(pokemonName);
    _lastPokemon = result.Data;
    _lastResponse = result.Response;
}

[Then(@"the pokemon should have ""(.*)"" type")]
public async Task ThenThePokemonShouldHaveType(string expectedType)
{
    var hasType = _lastPokemon!.Types.Any(t => t.Type.Name == expectedType);
    Assert.That(hasType, Is.True);
}
```

**Complete Files**:
- Client: `Tests/API/Clients/PokeApiClient.cs`
- Model: `Tests/API/Models/Pokemon.cs`
- Question: `Tests/API/Questions/ThePokemon.cs` ✨

### Example 3: Unit Testing (Framework Components)

**Feature**: `Tests/Features/UnitFeatures/screenplay_pattern_tests.feature`
```gherkin
@allure.parentSuite:Unit-Tests
@allure.suite:Framework-Components
Feature: Screenplay Pattern Components Unit Tests
  
  @unit @smoke
  Scenario: TheText Question returns correct element text
    Given I have a page with a text element
    When I ask TheText question for that element
    Then the question should return the correct text content
```

**Step Bindings**: `Tests/Tests/StepBindings/Unit/ScreenplayPatternStepBindings.cs`

---

## ⚠️ Anti-Patterns

### ❌ Don't: Put step bindings in wrong location

```
Tests/UI/StepBindings/  ← WRONG
Tests/StepBindings/     ← WRONG
```

### ✅ Do: Use correct location

```
Tests/Tests/StepBindings/Ui/  ← CORRECT
```

### ❌ Don't: Use magic strings in step bindings

```csharp
await Actor!.Page.ClickAsync("#submit");  // ❌ Magic string
```

### ✅ Do: Use centralized locators

```csharp
await Actor!.Page.ClickAsync(LoginLocators.SubmitButton);  // ✅
```

### ❌ Don't: Mix concerns in Page Objects

```csharp
public async Task LoginAndVerify(string user, string pass)
{
    await LoginAsync(user, pass);
    Assert.That(await IsLoggedIn(), Is.True);  // ❌ Assertion in PO
}
```

### ✅ Do: Separate actions and assertions

```csharp
// Page Object - Actions only
public async Task LoginAsync(string user, string pass) { }

// Step Binding - Assertions with Questions
var isLoggedIn = await Actor!.Asks(TheVisibility.Of(".user-menu"));
Assert.That(isLoggedIn, Is.True);
```

### ❌ Don't: Use direct Playwright calls in step bindings

```csharp
await Actor!.Page.FillAsync("#username", "test");  // ❌ Direct call
```

### ✅ Do: Use Tasks for actions

```csharp
await Actor!.AttemptsTo(LoginToApplication.WithCredentials("test", "pass"));
```

### ❌ Don't: Create Questions without proper naming

```csharp
public class TextQuestion : IQuestion<string>  // ❌ Generic name
public class GetText : IQuestion<string>       // ❌ Doesn't follow pattern
```

### ✅ Do: Follow The{Property} naming pattern

```csharp
public class TheText : IQuestion<string>       // ✅
public class TheVisibility : IQuestion<bool>   // ✅
public class TheTodoItems : IQuestion<List<string>>  // ✅
```

---

## 🤖 AI Agent Instructions

When generating code for QuantumTest-Suite:

1. **Always use correct paths** from the directory structure above
2. **Follow naming conventions** exactly as specified
3. **Use Screenplay Pattern** - Never bypass Tasks for actions
4. **Use Questions Pattern** ✨ - Always use Questions for data retrieval in assertions
5. **Include all required using statements** in step bindings
6. **Inherit from correct base classes**: `UiStepBindingsBase` or `ApiStepBindingsBase`
7. **Use Allure attributes** for reporting: `[AllureParentSuite]`, `[AllureSuite]`, `[AllureFeature]`
8. **Make all I/O operations async** with proper `await`
9. **Use centralized locators** - Never use magic strings
10. **Follow VerbNoun pattern** for Task naming

### Output Format

When generating files, provide:

```markdown
=== FILE STRUCTURE ===

1. **Feature File**: `Tests/Features/UiFeatures/feature_name.feature`
   Description: [What this feature tests]

2. **Step Bindings**: `Tests/Tests/StepBindings/Ui/FeatureNameStepBindings.cs`
   Description: [Step definitions with Screenplay Pattern]

3. **Page Object**: `Tests/UI/Pages/PageNamePage.cs`
   Description: [Page interactions]

4. **Locators**: `Tests/UI/Locators/PageNameLocators.cs`
   Description: [Centralized element selectors]

5. **Tasks**: `Tests/UI/Screenplay/Tasks/TaskName.cs`
   Description: [High-level business actions]

6. **Questions** ✨: `Tests/UI/Screenplay/Questions/ThePropertyName.cs`
   Description: [Data retrieval for assertions]

Dependencies: [List files that depend on each other]
```

---

## 📞 Support

For questions about this guide or the framework:

- Architecture questions: See `docs/ARCHITECTURE.md`
- Abilities guide: See `docs/ABILITIES-GUIDE.md`
- Questions guide: See `docs/QUESTIONS-GUIDE.md` ✨
- Allure reporting: See `docs/ALLURE-QUICKSTART.md`

---

**End of File Structure Guide v2.0.0**
│   │   │
│   │   ├── Locators/                 # Centralized locators (CSS/XPath)
│   │   │   ├── *Locators.cs         # e.g., SauceDemoLocators.cs
│   │   │   └── Pattern: [PageName]Locators.cs
│   │   │
│   │   ├── Screenplay/               # Screenplay Pattern components
│   │   │   ├── Actors/               # Actor implementations
│   │   │   │   └── Actor.cs          # Main Actor class
│   │   │   │
│   │   │   ├── Abilities/            # Actor capabilities
│   │   │   │   ├── IAbility.cs       # Base interface
│   │   │   │   ├── RememberData.cs   # State management
│   │   │   │   ├── AccessDatabase.cs # DB access
│   │   │   │   ├── CallApiEndpoint.cs # API calls
│   │   │   │   └── ReadConfiguration.cs # Config access
│   │   │   │
│   │   │   ├── Tasks/                # High-level business actions
│   │   │   │   ├── ITask.cs          # Base interface
│   │   │   │   ├── LoginToSauceDemo.cs
│   │   │   │   ├── SearchGitHubUser.cs
│   │   │   │   └── SubmitUltimateQaForm.cs
│   │   │   │   └── Pattern: [Action][Context].cs
│   │   │   │
│   │   │   └── Questions/            # ⚠️ TO BE IMPLEMENTED
│   │   │       └── IQuestion.cs      # Query patterns for assertions
│   │   │       └── Pattern: The[Property].cs
│   │   │       └── Examples:
│   │   │           ├── TheDisplayedInventory.cs
│   │   │           ├── TheCurrentUrl.cs
│   │   │           └── TheVisibleText.cs
│   │   │
│   │   └── Drivers/                  # Browser driver management
│   │       └── PlaywrightDriver.cs
│   │
│   ├── API/                          # API test infrastructure
│   │   ├── Clients/                  # API client wrappers
│   │   ├── Models/                   # Request/Response DTOs
│   │   └── Services/                 # Business logic for API
│   │
│   ├── Core/                         # Shared utilities
│   │   ├── Config/                   # Configuration management
│   │   ├── Extensions/               # Extension methods
│   │   └── Utilities/                # Helper utilities
│   │
│   └── Data/                         # Test data management
│       ├── Factories/                # Data factories (Bogus)
│       └── TestData/                 # Static test data
│
├── Core/                             # ❌ OLD STRUCTURE - Being migrated
│   └── PageObjects/                  # Legacy location
│       └── *Page.cs                  # Use Tests/UI/Pages/ instead
│
└── docs/                             # Framework documentation
    ├── FILE-STRUCTURE-GUIDE.md      # This file
    ├── ARCHITECTURE.md
    ├── ABILITIES-GUIDE.md
    └── *.md

```

---

## 🎯 File Naming Conventions

### 1. Feature Files
- **Location**: `Tests/Features/UiFeatures/` or `Tests/Features/ApiFeatures/`
- **Pattern**: `snake_case.feature`
- **Examples**:
  - `saucedemo_login.feature`
  - `github_user_search.feature`
  - `restful_booker_crud.feature`

### 2. Step Bindings (Step Definitions)
- **Location**: `Tests/Tests/StepBindings/Ui/` or `Tests/Tests/StepBindings/Api/`
- **Pattern**: `[FeatureName]StepBindings.cs` (PascalCase)
- **Examples**:
  - `SauceDemoStepBindings.cs`
  - `GitHubUserSearchStepBindings.cs`
  - `RestfulBookerStepBindings.cs`

### 3. Page Objects
- **Location**: `Tests/UI/Pages/`
- **Pattern**: `[PageName]Page.cs` (PascalCase + "Page" suffix)
- **Examples**:
  - `SauceDemoLoginPage.cs`
  - `GoogleSearchPage.cs`
  - `PlaywrightOfficialPage.cs`

### 4. Locators
- **Location**: `Tests/UI/Locators/`
- **Pattern**: `[PageName]Locators.cs` (PascalCase + "Locators" suffix)
- **Examples**:
  - `SauceDemoLocators.cs`
  - `GoogleLocators.cs`
  - `PlaywrightLocators.cs`

### 5. Screenplay Tasks
- **Location**: `Tests/UI/Screenplay/Tasks/`
- **Pattern**: `[Verb][Context].cs` (Action-oriented names)
- **Examples**:
  - `LoginToSauceDemo.cs`
  - `SearchGitHubUser.cs`
  - `SubmitUltimateQaForm.cs`
  - `NavigateToGoogle.cs`

### 6. Screenplay Abilities
- **Location**: `Tests/UI/Screenplay/Abilities/`
- **Pattern**: `[Capability].cs` (Noun describing capability)
- **Examples**:
  - `RememberData.cs`
  - `AccessDatabase.cs`
  - `CallApiEndpoint.cs`
  - `ReadConfiguration.cs`

### 7. Screenplay Questions (TO IMPLEMENT)
- **Location**: `Tests/UI/Screenplay/Questions/`
- **Pattern**: `The[Property].cs` (Question pattern)
- **Examples**:
  - `TheDisplayedInventory.cs`
  - `TheCurrentUrl.cs`
  - `TheVisibleText.cs`
  - `TheElementState.cs`

---

## 📋 STANDARD FILE STRUCTURE TEMPLATES

### Template 1: Simple UI Test (Screenplay Pattern)

**Use when**: Basic UI navigation and interaction

```
=== FILE STRUCTURE ===
FILE: Tests/Features/UiFeatures/[feature_name].feature
DESCRIPTION: Gherkin feature file with UI test scenarios
DEPENDENCIES: None
---
FILE: Tests/Tests/StepBindings/Ui/[FeatureName]StepBindings.cs
DESCRIPTION: Step definitions using Screenplay Pattern
DEPENDENCIES: Tests/UI/Pages/*Page.cs, Tests/UI/Screenplay/Tasks/*.cs, Reqnroll, NUnit.Framework
---
FILE: Tests/UI/Pages/[PageName]Page.cs
DESCRIPTION: Page Object Model for page interactions
DEPENDENCIES: Tests/UI/Locators/[PageName]Locators.cs, Microsoft.Playwright
---
FILE: Tests/UI/Locators/[PageName]Locators.cs
DESCRIPTION: Centralized locators for page elements
DEPENDENCIES: None
---
FILE: Tests/UI/Screenplay/Tasks/[Action][Context].cs
DESCRIPTION: High-level business task
DEPENDENCIES: Tests/UI/Pages/*Page.cs, Tests/UI/Screenplay/Actors/Actor.cs
=== END FILE STRUCTURE ===
```

### Template 2: Complex UI Test with Multiple Pages

**Use when**: Multi-page flows, navigation between pages

```
=== FILE STRUCTURE ===
FILE: Tests/Features/UiFeatures/[feature_name].feature
DESCRIPTION: Gherkin feature file with complex UI workflow
DEPENDENCIES: None
---
FILE: Tests/Tests/StepBindings/Ui/[FeatureName]StepBindings.cs
DESCRIPTION: Step definitions orchestrating multiple tasks
DEPENDENCIES: Tests/UI/Screenplay/Tasks/*.cs, Tests/UI/Screenplay/Abilities/*.cs
---
FILE: Tests/UI/Pages/[FirstPage]Page.cs
DESCRIPTION: Page Object for first page
DEPENDENCIES: Tests/UI/Locators/[FirstPage]Locators.cs
---
FILE: Tests/UI/Locators/[FirstPage]Locators.cs
DESCRIPTION: Locators for first page
DEPENDENCIES: None
---
FILE: Tests/UI/Pages/[SecondPage]Page.cs
DESCRIPTION: Page Object for second page
DEPENDENCIES: Tests/UI/Locators/[SecondPage]Locators.cs
---
FILE: Tests/UI/Locators/[SecondPage]Locators.cs
DESCRIPTION: Locators for second page
DEPENDENCIES: None
---
FILE: Tests/UI/Screenplay/Tasks/[Action][Context].cs
DESCRIPTION: Business task coordinating page interactions
DEPENDENCIES: Tests/UI/Pages/*Page.cs, Tests/UI/Screenplay/Actors/Actor.cs
=== END FILE STRUCTURE ===
```

### Template 3: API Test

**Use when**: Testing REST APIs

```
=== FILE STRUCTURE ===
FILE: Tests/Features/ApiFeatures/[feature_name].feature
DESCRIPTION: Gherkin feature file for API test
DEPENDENCIES: None
---
FILE: Tests/Tests/StepBindings/Api/[FeatureName]StepBindings.cs
DESCRIPTION: Step definitions for API testing
DEPENDENCIES: Tests/API/Clients/*.cs, Tests/API/Models/*.cs
---
FILE: Tests/API/Models/[Entity]Request.cs
DESCRIPTION: Request DTO model
DEPENDENCIES: None
---
FILE: Tests/API/Models/[Entity]Response.cs
DESCRIPTION: Response DTO model
DEPENDENCIES: None
=== END FILE STRUCTURE ===
```

---

## 💻 CODE STRUCTURE REQUIREMENTS

### 1. Feature File Format (.feature)

```gherkin
@allure.parentSuite:UI-Tests
@allure.suite:[SuiteName]
@allure.feature:[FeatureName]
@allure.owner:[TeamName]
Feature: [Feature Title]
  [Feature description]

  @ui @smoke
  @allure.story:[StoryName]
  @allure.severity:critical
  Scenario: [Scenario name]
    Given [precondition]
    When [action]
    Then [expected result]
```

### 2. Step Bindings Class Template

```csharp
using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Tests.StepBindings.Base;
using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Tasks;
using Reqnroll;

namespace QuantumTestSuite.Tests.StepBindings.Ui;

/// <summary>
/// Step bindings for [Feature Name]
/// </summary>
[Binding]
[AllureParentSuite("UI Tests")]
[AllureSuite("[Suite Name]")]
[AllureFeature("[Feature Name]")]
public class [FeatureName]StepBindings : UiStepBindingsBase
{
    public [FeatureName]StepBindings(
        ScenarioContext scenarioContext,
        AppSettings settings)
        : base(scenarioContext, settings)
    {
    }

    [Given(@"[regex pattern]")]
    public async Task Given[MethodName]()
    {
        await ExecuteGivenAsync(async () =>
        {
            await EnsureActorAsync("User");
            // Implementation
        });
    }

    [When(@"[regex pattern]")]
    public async Task When[MethodName]()
    {
        await ExecuteWhenAsync(async () =>
        {
            await EnsureActorAsync("User");
            await Actor!.AttemptsTo(new [TaskName](params));
        });
    }

    [Then(@"[regex pattern]")]
    public async Task Then[MethodName]()
    {
        await ExecuteThenAsync(async () =>
        {
            // Assertions
            Assert.That(actual, Is.EqualTo(expected), "Error message");
        });
    }
}
```

### 3. Page Object Class Template

```csharp
using Microsoft.Playwright;
using QuantumTestSuite.UI.Locators;
using QuantumTestSuite.Core.Config;

namespace QuantumTestSuite.UI.Pages;

/// <summary>
/// Page Object for [Page Name] page
/// </summary>
public class [PageName]Page
{
    private readonly IPage _page;
    private readonly AppSettings? _settings;

    public [PageName]Page(IPage page, AppSettings? settings = null)
    {
        _page = page;
        _settings = settings;
    }

    public async Task NavigateAsync()
    {
        var url = _settings?.BaseUrl ?? "https://default-url.com";
        await _page.GotoAsync(url, new PageGotoOptions 
        { 
            WaitUntil = WaitUntilState.NetworkIdle 
        });
        await _page.WaitForSelectorAsync([PageName]Locators.MainElement);
    }

    public async Task [Action]Async(string param)
    {
        await _page.FillAsync([PageName]Locators.InputField, param);
        await _page.ClickAsync([PageName]Locators.SubmitButton);
    }

    public async Task<bool> Is[State]Async()
    {
        return await _page.IsVisibleAsync([PageName]Locators.TargetElement);
    }
}
```

### 4. Locators Class Template

```csharp
namespace QuantumTestSuite.UI.Locators;

/// <summary>
/// Centralized locators for [Page Name] page
/// </summary>
public static class [PageName]Locators
{
    // CSS Selectors (preferred)
    public const string MainElement = "css=selector";
    public const string InputField = "#input-id";
    public const string SubmitButton = "button[type='submit']";
    
    // XPath (when CSS not suitable)
    public const string DynamicElement = "xpath=//div[contains(@class, 'target')]";
    
    // Playwright auto-locators (best practice)
    public const string LoginButton = "text=Login";
    public const string UsernameInput = "input[name='username']";
}
```

### 5. Screenplay Task Template

```csharp
using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Actors;
using QuantumTestSuite.Core.Config;

namespace QuantumTestSuite.UI.Screenplay.Tasks;

/// <summary>
/// Task: [Action description]
/// </summary>
public class [Action][Context] : ITask
{
    private readonly string _param1;
    private readonly AppSettings? _settings;

    public [Action][Context](string param1, AppSettings? settings = null)
    {
        _param1 = param1;
        _settings = settings;
    }

    public async Task ExecuteAsync(Actor actor)
    {
        var page = new [PageName]Page(actor.Page, _settings);
        await page.NavigateAsync();
        await page.[Action]Async(_param1);
    }
}
```

### 6. ITask Interface (Reference Only - DO NOT CREATE)

```csharp
namespace QuantumTestSuite.UI.Screenplay.Tasks;

public interface ITask
{
    Task ExecuteAsync(Actor actor);
}
```

---

## ✅ VALIDATION RULES

### File Path Validation
- ✅ Feature files MUST be in `Tests/Features/UiFeatures/` or `Tests/Features/ApiFeatures/`
- ✅ Step bindings MUST be in `Tests/Tests/StepBindings/Ui/` or `Tests/Tests/StepBindings/Api/`
- ✅ Pages MUST be in `Tests/UI/Pages/`
- ✅ Locators MUST be in `Tests/UI/Locators/`
- ✅ Tasks MUST be in `Tests/UI/Screenplay/Tasks/`
- ❌ DO NOT use `Features/` root directory (deprecated)
- ❌ DO NOT use `Core/PageObjects/` (deprecated)
- ❌ DO NOT use `Tests/UI/` for step definitions

### Naming Validation
- ✅ Feature files: `snake_case.feature`
- ✅ Step bindings: `PascalCaseStepBindings.cs`
- ✅ Pages: `PascalCasePage.cs`
- ✅ Locators: `PascalCaseLocators.cs`
- ✅ Tasks: `VerbNoun.cs` (e.g., `LoginToSauceDemo`)
- ❌ DO NOT use hyphens or spaces in file names
- ❌ DO NOT mix naming conventions

### Dependency Validation
- ✅ Step bindings depend on Tasks/Pages
- ✅ Tasks depend on Pages
- ✅ Pages depend on Locators
- ✅ Locators have no dependencies
- ❌ DO NOT create circular dependencies
- ❌ DO NOT skip layers (StepBindings → Locators directly)

---

## 🚫 ANTI-PATTERNS (DO NOT DO THIS)

### ❌ Wrong: Feature in root directory
```
Features/google_search.feature  # DEPRECATED LOCATION
```

### ✅ Correct: Feature in Tests/Features/
```
Tests/Features/UiFeatures/google_search.feature
```

### ❌ Wrong: Step definitions in UI folder
```
Tests/UI/GoogleSearchSteps.cs  # WRONG LOCATION
```

### ✅ Correct: Step definitions in StepBindings
```
Tests/Tests/StepBindings/Ui/GoogleSearchStepBindings.cs
```

### ❌ Wrong: Page Objects in Core
```
Core/PageObjects/GooglePage.cs  # DEPRECATED
```

### ✅ Correct: Pages in Tests/UI/Pages
```
Tests/UI/Pages/GooglePage.cs
```

### ❌ Wrong: Locators inside Page class
```csharp
public class GooglePage
{
    private const string SearchBox = "#search";  // BAD
}
```

### ✅ Correct: Centralized Locators
```csharp
// Tests/UI/Locators/GoogleLocators.cs
public static class GoogleLocators
{
    public const string SearchBox = "#search";
}
```

---

## 🤖 AI AGENT INSTRUCTIONS

### CRITICAL: Output Format for AI Agents

When you generate file structure, you MUST output in this EXACT format:

```
=== FILE STRUCTURE ===
FILE: Tests/Features/UiFeatures/[feature_name].feature
DESCRIPTION: [Clear description]
DEPENDENCIES: None
---
FILE: Tests/Tests/StepBindings/Ui/[FeatureName]StepBindings.cs
DESCRIPTION: [Clear description]
DEPENDENCIES: [Comma-separated list or "None"]
---
FILE: Tests/UI/Pages/[PageName]Page.cs
DESCRIPTION: [Clear description]
DEPENDENCIES: Tests/UI/Locators/[PageName]Locators.cs, Microsoft.Playwright
---
FILE: Tests/UI/Locators/[PageName]Locators.cs
DESCRIPTION: [Clear description]
DEPENDENCIES: None
---
FILE: Tests/UI/Screenplay/Tasks/[Action][Context].cs
DESCRIPTION: [Clear description]
DEPENDENCIES: Tests/UI/Pages/*Page.cs, Tests/UI/Screenplay/Actors/Actor.cs
=== END FILE STRUCTURE ===
```

---

*Last Updated: January 6, 2026*  
*Framework Version: .NET 8 | Reqnroll 2.x | Playwright 1.x*
