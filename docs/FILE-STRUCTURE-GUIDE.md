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
│   ├── Framework/                         # 🆕 Framework code (reusable components)
│   │   ├── API/                           # API testing components
│   │   │   ├── Clients/                   # HTTP clients for different APIs
│   │   │   │   └── *Client.cs             # PascalCase + Client suffix
│   │   │   ├── Models/                    # Response/request models
│   │   │   │   └── *.cs                   # PascalCase, matches API structure
│   │   │   ├── Questions/                 # ✨ API-specific questions
│   │   │   │   ├── IApiQuestion.cs        # Base interface for API questions
│   │   │   │   ├── TheResponseStatus.cs   # Check HTTP status codes
│   │   │   │   ├── TheResponseBody.cs     # Parse response body
│   │   │   │   └── ThePokemon.cs          # Domain-specific questions
│   │   │   ├── Helpers/                   # API helper utilities
│   │   │   │   ├── ApiRequestContextFactory.cs
│   │   │   │   └── ApiResponse.cs
│   │   │   └── Endpoints/                 # API endpoint definitions
│   │   │       └── ApiEndpoints.cs
│   │   │
│   │   ├── UI/                            # UI automation components
│   │   │   ├── Pages/                     # Page Object Model (POM)
│   │   │   │   └── *Page.cs               # PascalCase + Page suffix
│   │   │   ├── Locators/                  # Centralized element locators
│   │   │   │   └── *Locators.cs           # PascalCase + Locators suffix (static class)
│   │   │   ├── Screenplay/                # Screenplay Pattern implementation
│   │   │   │   ├── Actors/                # Actor class (who performs actions)
│   │   │   │   │   └── Actor.cs
│   │   │   │   ├── Abilities/             # Abilities (what actors can do)
│   │   │   │   │   ├── IAbility.cs        # Base interface
│   │   │   │   │   ├── BrowseTheWeb.cs    # Browser interactions
│   │   │   │   │   ├── RememberData.cs    # In-memory data storage
│   │   │   │   │   ├── AccessDatabase.cs  # Database operations
│   │   │   │   │   ├── CallApiEndpoint.cs # API interactions
│   │   │   │   │   └── ReadConfiguration.cs # Configuration access
│   │   │   │   ├── Tasks/                 # Tasks (high-level actions)
│   │   │   │   │   └── *.cs               # VerbNoun pattern (e.g., NavigateToPlaywrightDemo, AddTodoItem)
│   │   │   │   └── Questions/             # ✨ Questions (data retrieval & assertions)
│   │   │   │       ├── IQuestion.cs       # Generic question interface
│   │   │   │       ├── TheText.cs         # Get text from elements
│   │   │   │       ├── TheVisibility.cs   # Check element visibility
│   │   │   │       ├── TheValue.cs        # Get input values
│   │   │   │       ├── TheCount.cs        # Count elements
│   │   │   │       ├── TheTodoItems.cs    # Get todo list items
│   │   │   │       ├── TheCurrentUrl.cs   # Get current page URL
│   │   │   │       └── TheTitle.cs        # Get page title
│   │   │   └── Drivers/                   # Browser/driver management
│   │   │       └── PlaywrightDriver.cs
│   │   │
│   │   └── Core/                          # Core framework utilities
│   │       ├── Config/                    # Configuration management
│   │       │   ├── ConfigManager.cs
│   │       │   └── AppSettings.cs
│   │       ├── Context/                   # Test context (dependency injection)
│   │       │   └── TestContexts.cs
│   │       ├── DependencyInjection/       # DI container configuration
│   │       │   └── DependencyInjectionConfig.cs
│   │       ├── Hooks/                     # Test lifecycle hooks
│   │       │   └── TestHooks.cs
│   │       ├── Logging/                   # Logging infrastructure
│   │       │   └── ConsoleLogger.cs
│   │       ├── Models/                    # Shared models
│   │       │   └── TestModels.cs
│   │       ├── Reporting/                 # Allure reporting
│   │       │   ├── AllureHelper.cs
│   │       │   ├── AllureEnvironmentWriter.cs
│   │       │   ├── AllureCategoriesWriter.cs
│   │       │   └── AllureExecutorWriter.cs
│   │       ├── Services/                  # Shared services
│   │       │   ├── IApiServices.cs
│   │       │   └── ApiServices.cs
│   │       └── Utilities/                 # Helper utilities
│   │           ├── RetryHelper.cs
│   │           ├── WaitHelper.cs
│   │           ├── ContextFactory.cs
│   │           └── TestTimeouts.cs
│   │
│   ├── StepBindings/                      # 🆕 Step definitions (Reqnroll bindings)
│   │   └── StepBindings/
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
│   ├── TestData/                          # 🆕 Test data (renamed from Data)
│   │   ├── Factories/                     # Data factories (test data generation)
│   │   │   ├── BookingFactory.cs
│   │   │   └── UserFactory.cs
│   │   └── DataProviders/                 # Data providers (external data sources)
│   │       └── JsonDataProvider.cs
│   │
│   ├── Reports/                           # Generated test reports
│   │   └── allure-results/                # Allure report output
│   │
│   └── appsettings.json                   # Environment configuration
│
├── docs/                                  # Documentation
│   ├── ARCHITECTURE.md                    # Framework architecture overview
│   ├── FILE-STRUCTURE-GUIDE.md            # This document
│   ├── STEP-BINDINGS-ARCHITECTURE.md      # Step bindings architecture
│   ├── FRAMEWORK_STATUS.md                # Framework quality rating
│   └── REFACTOR_PROPOSAL.md               # Refactoring documentation
│
└── QuantumTestSuite.Tests.csproj          # Project file

```

### ✨ Changes in Version 2.0

**Directory Restructuring (January 2026)**:

1. **Framework/** - New folder grouping all reusable framework code:
   - `Tests/API` → `Tests/Framework/API` (namespace: `QuantumTestSuite.Framework.API`)
   - `Tests/Core` → `Tests/Framework/Core` (namespace: `QuantumTestSuite.Framework.Core`)
   - `Tests/UI` → `Tests/Framework/UI` (namespace: `QuantumTestSuite.Framework.UI`)

2. **StepBindings/** - Renamed from `Tests/Tests` for clarity:
   - `Tests/Tests` → `Tests/StepBindings` (namespace: `QuantumTestSuite.StepBindings`)

3. **TestData/** - Renamed from `Data` for better semantics:
   - `Tests/Data` → `Tests/TestData` (namespace: `QuantumTestSuite.TestData`)

**Benefits**:
- ✅ Clear separation between framework code and test code
- ✅ Eliminates confusing "Tests/Tests" nesting
- ✅ More intuitive navigation and onboarding
- ✅ Professional structure aligned with industry standards
- ✅ Preserves all functionality (Allure, hooks, logging)

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

**Location**: `Tests/StepBindings/StepBindings/{Ui|Api|Unit}/`  
**Naming**: `PascalCase` + `StepBindings.cs` suffix  
**Examples**:
- ✅ `PlaywrightDemoStepBindings.cs`
- ✅ `PokemonApiStepBindings.cs`
- ✅ `ScreenplayPatternStepBindings.cs`
- ❌ `PlaywrightDemo.cs` (missing suffix)
- ❌ `playwright_demo_step_bindings.cs` (wrong case)

### Page Objects

**Location**: `Tests/Framework/UI/Pages/`  
**Naming**: `PascalCase` + `Page.cs` suffix  
**Examples**:
- ✅ `PlaywrightDemoPage.cs`
- ✅ `LoginPage.cs`
- ❌ `PlaywrightDemo.cs` (missing suffix)

### Locators

**Location**: `Tests/Framework/UI/Locators/`  
**Naming**: `PascalCase` + `Locators.cs` suffix  
**Must be**: Static class with const string fields  
**Examples**:
- ✅ `PlaywrightDemoLocators.cs`
- ✅ `LoginLocators.cs`

### Tasks

**Location**: `Tests/Framework/UI/Screenplay/Tasks/`  
**Naming**: `VerbNoun` pattern in `PascalCase`  
**Examples**:
- ✅ `AddTodoItem.cs`
- ✅ `NavigateToPlaywrightDemo.cs`
- ✅ `LoginToApplication.cs`
- ❌ `TodoItem.cs` (missing verb)

### Questions (NEW) ✨

**Location**: 
- UI: `Tests/Framework/UI/Screenplay/Questions/`
- API: `Tests/Framework/API/Questions/`

**Naming**: `The{Property}.cs` pattern  
**Examples**:
- ✅ `TheText.cs` - Retrieves text content
- ✅ `TheVisibility.cs` - Checks visibility
- ✅ `TheResponseStatus.cs` - Gets HTTP status
- ✅ `TheTodoItems.cs` - Custom question

### API Clients

**Location**: `Tests/Framework/API/Clients/`  
**Naming**: `PascalCase` + `Client.cs` suffix  
**Examples**:
- ✅ `PokeApiClient.cs`
- ✅ `GitHubClient.cs`

---

## 🎭 Screenplay Pattern Quick Reference

The Screenplay Pattern has 4 layers. For detailed philosophy, see [ARCHITECTURE.md](ARCHITECTURE.md).

### Pattern Structure

| Layer | Location | Purpose | Usage |
|-------|----------|---------|-------|
| **Actors** | `Tests/Framework/UI/Screenplay/Actors/` | Who performs actions | `var actor = new Actor("User", page)` |
| **Abilities** | `Tests/Framework/UI/Screenplay/Abilities/` | What actors can do | `actor.WhoCan(new RememberData())` |
| **Tasks** | `Tests/Framework/UI/Screenplay/Tasks/` | How they do it | `await actor.AttemptsTo(SomeTask.Do())` |
| **Questions** | `Tests/Framework/UI/Screenplay/Questions/`<br>`Tests/Framework/API/Questions/` | What they see | `var result = await actor.Asks(TheText.Of(".selector"))` |

### Key Methods
- `actor.AttemptsTo(task1, task2)` - Execute tasks
- `actor.Asks(question)` - Retrieve data for assertions
- `actor.Using<Ability>()` - Access abilities

**Best Practices**: See [STEP-BINDINGS-ARCHITECTURE.md](STEP-BINDINGS-ARCHITECTURE.md) for usage examples.

---

## 📦 Code Templates

> **Complete Templates**: See templates below. For additional examples (Feature files, Step Bindings, Questions), see [STEP-BINDINGS-ARCHITECTURE.md](STEP-BINDINGS-ARCHITECTURE.md).

### 1. API Client Template

**Location**: `Tests/Framework/API/Clients/*Client.cs`

```csharp
using System.Net.Http;
using System.Text.Json;
using QuantumTestSuite.API.Models;

namespace QuantumTestSuite.Framework.API.Clients;

public class ApiNameClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiNameClient(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient 
        { BaseAddress = new Uri("https://api.example.com/") };
        _jsonOptions = new JsonSerializerOptions
        { PropertyNameCaseInsensitive = true };
    }

    public async Task<(Model? Data, HttpResponseMessage Response)> GetResourceAsync(int id)
    {
        var response = await _httpClient.GetAsync($"resource/{id}");
        if (!response.IsSuccessStatusCode) return (null, response);
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<Model>(content, _jsonOptions);
        return (data, response);
    }
}
```

### 2. UI Page + Locators Template (Combined)

**Page Object**: `Tests/Framework/UI/Pages/*Page.cs`

```csharp
using Microsoft.Playwright;
using QuantumTestSuite.UI.Locators;

namespace QuantumTestSuite.Framework.UI.Pages;

public class PageNamePage
{
    private readonly IPage _page;
    public PageNamePage(IPage page) => _page = page;

    public async Task NavigateAsync() => 
        await _page.GotoAsync("https://example.com", new() { WaitUntil = WaitUntilState.NetworkIdle });
    public async Task<bool> IsVisibleAsync() => 
        await _page.IsVisibleAsync(PageNameLocators.MainContainer);
}
```

**Locators**: `Tests/Framework/UI/Locators/*Locators.cs`

```csharp
namespace QuantumTestSuite.Framework.UI.Locators;

public static class PageNameLocators
{
    public const string MainContainer = ".main-container";
    public const string UsernameInput = "input[name='username']";
    public const string SubmitButton = "button[type='submit']";
}
```

### 3. Task Template

**Location**: `Tests/Framework/UI/Screenplay/Tasks/*.cs`

```csharp
using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Actors;

namespace QuantumTestSuite.Framework.UI.Screenplay.Tasks;

public class TaskName : ITask
{
    private readonly string _param;
    private TaskName(string param) => _param = param;
    public static TaskName With(string param) => new(param);

    public async Task ExecuteAsync(Actor actor)
    {
        var page = new SomePage(actor.Page);
        await page.NavigateAsync();
        await page.DoSomethingAsync(_param);
    }
}
```

**Additional Templates**: For Step Bindings, Questions (UI/API), Feature files, see [STEP-BINDINGS-ARCHITECTURE.md](STEP-BINDINGS-ARCHITECTURE.md) sections on "Adding New Step Bindings" and "Creating Custom Questions".

---

## ✅ Validation Rules

### Critical Paths
- ✅ `Tests/Features/{UiFeatures|ApiFeatures}/` - Gherkin features
- ✅ `Tests/StepBindings/StepBindings/{Ui|Api}/` - Step definitions
- ✅ `Tests/Framework/UI/{Pages|Locators|Screenplay/Tasks|Screenplay/Questions}/`
- ✅ `Tests/Framework/API/{Clients|Questions}/`
- ❌ `Features/`, `Core/PageObjects/` - Deprecated

### Naming Conventions
- Feature files: `snake_case.feature`
- Step bindings: `*StepBindings.cs`
- Pages: `*Page.cs`, Locators: `*Locators.cs` (static)
- Tasks: `VerbNoun.cs`, Questions: `The*.cs`

### Dependencies
- Step bindings: Inherit `UiStepBindingsBase` or `ApiStepBindingsBase`
- Pages → Locators → (no dependencies)
- Tasks → Pages, Questions → (read-only)

---

## 📚 Examples

### Complete UI Feature: TodoMVC with Questions Pattern

**Feature**: `Tests/Features/UiFeatures/playwright_demo.feature`
```gherkin
@ui @smoke
Scenario: Add and verify items in todo list
  Given the user navigates to the Playwright demo page
  When the user adds "Buy groceries" to the list
  Then the list should contain 1 item
  And the list should include "Buy groceries"
```

**Step Bindings**: `Tests/StepBindings/StepBindings/Ui/PlaywrightDemoStepBindings.cs`
```csharp
[When(@"the user adds ""(.*)"" to the list")]
public async Task WhenTheUserAddsItemToTheList(string itemText)
{
    await ExecuteWhenAsync(async () =>
    {
        await Actor!.AttemptsTo(AddTodoItem.With(itemText)); // Task
    });
}

[Then(@"the list should contain (.*) items")]
public async Task ThenTheListShouldContainItems(int expectedCount)
{
    await ExecuteThenAsync(async () =>
    {
        // ✨ Questions Pattern for assertions
        var actualCount = await Actor!.Asks(TheCount.Of(PlaywrightDemoLocators.TodoItem));
        Assert.That(actualCount, Is.EqualTo(expectedCount));
    });
}
```

**Complete Files**:
- `Tests/Framework/UI/Pages/PlaywrightDemoPage.cs`
- `Tests/Framework/UI/Locators/PlaywrightDemoLocators.cs`
- `Tests/Framework/UI/Screenplay/Tasks/AddTodoItem.cs`
- `Tests/Framework/UI/Screenplay/Questions/TheCount.cs`, `TheTodoItems.cs`

**More Examples**: API testing (Pokemon), Unit testing (Framework), see [STEP-BINDINGS-ARCHITECTURE.md](STEP-BINDINGS-ARCHITECTURE.md) sections "Usage Example" and "Best Practices".

---

## ⚠️ Anti-Patterns

### 1. ❌ Magic Strings → ✅ Centralized Locators
```csharp
// ❌ Don't
await Actor!.Page.ClickAsync("#submit");

// ✅ Do
await Actor!.Page.ClickAsync(LoginLocators.SubmitButton);
```

### 2. ❌ Direct Playwright Calls → ✅ Tasks/Questions
```csharp
// ❌ Don't (in step bindings)
await Actor!.Page.FillAsync("#username", "test");
var count = await Actor!.Page.Locator(".item").CountAsync();

// ✅ Do
await Actor!.AttemptsTo(LoginToApplication.WithCredentials("test", "pass"));
var count = await Actor!.Asks(TheCount.Of(".item"));
```

### 3. ❌ Assertions in Page Objects → ✅ Questions in Step Bindings
```csharp
// ❌ Don't (in Page Object)
public async Task LoginAndVerify(string user, string pass)
{
    await LoginAsync(user, pass);
    Assert.That(await IsLoggedIn(), Is.True);  // Wrong layer
}

// ✅ Do (separate concerns)
// Page: Actions only
public async Task LoginAsync(string user, string pass) { }

// Step Binding: Assertions with Questions
var isLoggedIn = await Actor!.Asks(TheVisibility.Of(".user-menu"));
Assert.That(isLoggedIn, Is.True);
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

2. **Step Bindings**: `Tests/StepBindings/StepBindings/Ui/FeatureNameStepBindings.cs`
   Description: [Step definitions with Screenplay Pattern]

3. **Page Object**: `Tests/Framework/UI/Pages/PageNamePage.cs`
   Description: [Page interactions]

4. **Locators**: `Tests/Framework/UI/Locators/PageNameLocators.cs`
   Description: [Centralized element selectors]

5. **Tasks**: `Tests/Framework/UI/Screenplay/Tasks/TaskName.cs`
   Description: [High-level business actions]

6. **Questions** ✨: `Tests/Framework/UI/Screenplay/Questions/ThePropertyName.cs`
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
│       └── *Page.cs                  # Use Tests/Framework/UI/Pages/ instead
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
- **Location**: `Tests/StepBindings/StepBindings/Ui/` or `Tests/StepBindings/StepBindings/Api/`
- **Pattern**: `[FeatureName]StepBindings.cs` (PascalCase)
- **Examples**:
  - `SauceDemoStepBindings.cs`
  - `GitHubUserSearchStepBindings.cs`
  - `RestfulBookerStepBindings.cs`

### 3. Page Objects
- **Location**: `Tests/Framework/UI/Pages/`
- **Pattern**: `[PageName]Page.cs` (PascalCase + "Page" suffix)
- **Examples**:
  - `SauceDemoLoginPage.cs`
  - `GoogleSearchPage.cs`
  - `PlaywrightOfficialPage.cs`

### 4. Locators
- **Location**: `Tests/Framework/UI/Locators/`
- **Pattern**: `[PageName]Locators.cs` (PascalCase + "Locators" suffix)
- **Examples**:
  - `SauceDemoLocators.cs`
  - `GoogleLocators.cs`
  - `PlaywrightLocators.cs`

### 5. Screenplay Tasks
- **Location**: `Tests/Framework/UI/Screenplay/Tasks/`
- **Pattern**: `[Verb][Context].cs` (Action-oriented names)
- **Examples**:
  - `LoginToSauceDemo.cs`
  - `SearchGitHubUser.cs`
  - `SubmitUltimateQaForm.cs`
  - `NavigateToGoogle.cs`

### 6. Screenplay Abilities
- **Location**: `Tests/Framework/UI/Screenplay/Abilities/`
- **Pattern**: `[Capability].cs` (Noun describing capability)
- **Examples**:
  - `RememberData.cs`
  - `AccessDatabase.cs`
  - `CallApiEndpoint.cs`
  - `ReadConfiguration.cs`

### 7. Screenplay Questions (TO IMPLEMENT)
- **Location**: `Tests/Framework/UI/Screenplay/Questions/`
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
FILE: Tests/StepBindings/StepBindings/Ui/[FeatureName]StepBindings.cs
DESCRIPTION: Step definitions using Screenplay Pattern
DEPENDENCIES: Tests/Framework/UI/Pages/*Page.cs, Tests/Framework/UI/Screenplay/Tasks/*.cs, Reqnroll, NUnit.Framework
---
FILE: Tests/Framework/UI/Pages/[PageName]Page.cs
DESCRIPTION: Page Object Model for page interactions
DEPENDENCIES: Tests/Framework/UI/Locators/[PageName]Locators.cs, Microsoft.Playwright
---
FILE: Tests/Framework/UI/Locators/[PageName]Locators.cs
DESCRIPTION: Centralized locators for page elements
DEPENDENCIES: None
---
FILE: Tests/Framework/UI/Screenplay/Tasks/[Action][Context].cs
DESCRIPTION: High-level business task
DEPENDENCIES: Tests/Framework/UI/Pages/*Page.cs, Tests/Framework/UI/Screenplay/Actors/Actor.cs
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
FILE: Tests/StepBindings/StepBindings/Ui/[FeatureName]StepBindings.cs
DESCRIPTION: Step definitions orchestrating multiple tasks
DEPENDENCIES: Tests/Framework/UI/Screenplay/Tasks/*.cs, Tests/Framework/UI/Screenplay/Abilities/*.cs
---
FILE: Tests/Framework/UI/Pages/[FirstPage]Page.cs
DESCRIPTION: Page Object for first page
DEPENDENCIES: Tests/Framework/UI/Locators/[FirstPage]Locators.cs
---
FILE: Tests/Framework/UI/Locators/[FirstPage]Locators.cs
DESCRIPTION: Locators for first page
DEPENDENCIES: None
---
FILE: Tests/Framework/UI/Pages/[SecondPage]Page.cs
DESCRIPTION: Page Object for second page
DEPENDENCIES: Tests/Framework/UI/Locators/[SecondPage]Locators.cs
---
FILE: Tests/Framework/UI/Locators/[SecondPage]Locators.cs
DESCRIPTION: Locators for second page
DEPENDENCIES: None
---
FILE: Tests/Framework/UI/Screenplay/Tasks/[Action][Context].cs
DESCRIPTION: Business task coordinating page interactions
DEPENDENCIES: Tests/Framework/UI/Pages/*Page.cs, Tests/Framework/UI/Screenplay/Actors/Actor.cs
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
FILE: Tests/StepBindings/StepBindings/Api/[FeatureName]StepBindings.cs
DESCRIPTION: Step definitions for API testing
DEPENDENCIES: Tests/Framework/API/Clients/*.cs, Tests/Framework/API/Models/*.cs
---
FILE: Tests/Framework/API/Models/[Entity]Request.cs
DESCRIPTION: Request DTO model
DEPENDENCIES: None
---
FILE: Tests/Framework/API/Models/[Entity]Response.cs
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

namespace QuantumTestSuite.StepBindings.Ui;

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

namespace QuantumTestSuite.Framework.UI.Pages;

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
namespace QuantumTestSuite.Framework.UI.Locators;

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

namespace QuantumTestSuite.Framework.UI.Screenplay.Tasks;

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
namespace QuantumTestSuite.Framework.UI.Screenplay.Tasks;

public interface ITask
{
    Task ExecuteAsync(Actor actor);
}
```

---

## ✅ VALIDATION RULES

### File Path Validation
- ✅ Feature files MUST be in `Tests/Features/UiFeatures/` or `Tests/Features/ApiFeatures/`
- ✅ Step bindings MUST be in `Tests/StepBindings/StepBindings/Ui/` or `Tests/StepBindings/StepBindings/Api/`
- ✅ Pages MUST be in `Tests/Framework/UI/Pages/`
- ✅ Locators MUST be in `Tests/Framework/UI/Locators/`
- ✅ Tasks MUST be in `Tests/Framework/UI/Screenplay/Tasks/`
- ❌ DO NOT use `Features/` root directory (deprecated)
- ❌ DO NOT use `Core/PageObjects/` (deprecated)
- ❌ DO NOT use `Tests/Framework/UI/` for step definitions

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
Tests/Framework/UI/GoogleSearchSteps.cs  # WRONG LOCATION
```

### ✅ Correct: Step definitions in StepBindings
```
Tests/StepBindings/StepBindings/Ui/GoogleSearchStepBindings.cs
```

### ❌ Wrong: Page Objects in Core
```
Core/PageObjects/GooglePage.cs  # DEPRECATED
```

### ✅ Correct: Pages in Tests/Framework/UI/Pages
```
Tests/Framework/UI/Pages/GooglePage.cs
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
// Tests/Framework/UI/Locators/GoogleLocators.cs
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
FILE: Tests/StepBindings/StepBindings/Ui/[FeatureName]StepBindings.cs
DESCRIPTION: [Clear description]
DEPENDENCIES: [Comma-separated list or "None"]
---
FILE: Tests/Framework/UI/Pages/[PageName]Page.cs
DESCRIPTION: [Clear description]
DEPENDENCIES: Tests/Framework/UI/Locators/[PageName]Locators.cs, Microsoft.Playwright
---
FILE: Tests/Framework/UI/Locators/[PageName]Locators.cs
DESCRIPTION: [Clear description]
DEPENDENCIES: None
---
FILE: Tests/Framework/UI/Screenplay/Tasks/[Action][Context].cs
DESCRIPTION: [Clear description]
DEPENDENCIES: Tests/Framework/UI/Pages/*Page.cs, Tests/Framework/UI/Screenplay/Actors/Actor.cs
=== END FILE STRUCTURE ===
```

---

*Last Updated: January 6, 2026*  
*Framework Version: .NET 8 | Reqnroll 2.x | Playwright 1.x*
