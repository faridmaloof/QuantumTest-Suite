# Architecture - Quantum Test Suite

## Overview

Quantum Test Suite is an enterprise-grade test automation framework built on .NET 8, implementing BDD with Reqnroll (SpecFlow successor), UI/API testing with Playwright, and comprehensive reporting with Allure.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                         TEST FEATURES                            │
│                      (Gherkin/Reqnroll)                          │
└────────────────────────┬────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│                      STEP DEFINITIONS                            │
│              (StepBindings with DI injection)                    │
└───────┬──────────────────────────────────────────┬──────────────┘
        │                                           │
        ▼                                           ▼
┌──────────────────┐                      ┌──────────────────────┐
│  SERVICE LAYER   │                      │  SCREENPLAY PATTERN  │
│  (API Services)  │                      │   (4-Layer Model)    │
│                  │                      ├──────────────────────┤
│                  │                      │ • Actors (Who)       │
│                  │                      │ • Abilities (What)   │
│                  │                      │ • Tasks (How)        │
│                  │                      │ • Questions (See) ✨ │
└────────┬─────────┘                      └──────────┬───────────┘
         │                                            │
         ▼                                            ▼
┌──────────────────┐                      ┌──────────────────────┐
│  API CLIENTS     │                      │    PAGES/LOCATORS    │
│  (RestClient)    │                      │   (Page Objects)     │
└────────┬─────────┘                      └──────────┬───────────┘
         │                                            │
         └────────────────┬───────────────────────────┘
                          ▼
          ┌───────────────────────────────┐
          │      PLAYWRIGHT ENGINE         │
          │   (Browser/API Automation)     │
          └───────────────┬───────────────┘
                          │
                          ▼
          ┌───────────────────────────────┐
          │      REPORTING LAYER           │
          │     (Allure + Artifacts)       │
          └───────────────────────────────┘
```

## Core Principles

### 1. Separation of Concerns
- **Features**: Business logic in Gherkin
- **Step Definitions**: Glue code (thin layer)
- **Services**: Business logic orchestration
- **Clients**: Technical implementation
- **Pages**: UI element abstraction

### 2. Dependency Injection
Framework uses Autofac container for:
- Service lifetime management
- Testability
- Decoupling
- Configuration injection

### 3. Type-Safe Context Management
Instead of string-based ScenarioContext keys:
```csharp
// ❌ Old way
_scenarioContext["lastResponse"] = response;

// ✅ New way
var context = _scenarioContext.Get<ApiTestContext>();
context.LastResponse = response;
```

## Design Patterns

### Factory Pattern
**Location**: `Data/Factories/`

**Purpose**: Generate test data dynamically
```csharp
var booking = BookingFactory.Create(); // Uses Bogus for realistic data
```

### Service Layer Pattern
**Location**: `Core/Services/`

**Purpose**: Encapsulate business logic
```csharp
public interface IBookingService
{
    Task<ApiResponse<BookingResponse>> CreateBookingAsync(BookingRequest request);
}
```

### Page Object Model (POM)
**Location**: `UI/Pages/` + `UI/Locators/`

**Purpose**: Encapsulate page structure
```csharp
public class SauceDemoLoginPage
{
    public async Task LoginAsync(string username, string password) { }
}
```

### Screenplay Pattern (4-Layer Model) ✨
**Location**: `UI/Screenplay/` + `API/Questions/`

**Purpose**: High-level user interactions following Actor-Ability-Task-Question model

#### Actors (Who performs actions)
**Location**: `UI/Screenplay/Actors/`

Represents users/systems performing actions:
```csharp
var actor = new Actor("QA Tester", page);
actor.Can(new BrowseTheWeb(page));
await actor.AttemptsTo(new LoginToSauceDemo("user", "pass"));
```

#### Abilities (What actors can do)
**Location**: `UI/Screenplay/Abilities/`

Core capabilities actors possess:
```csharp
// UI Abilities
actor.Can(new BrowseTheWeb(page));
actor.Can(new RememberData());

// API Abilities
actor.Can(new CallApiEndpoint(apiContext));

// Database Abilities
actor.Can(new AccessDatabase(connectionString));
```

#### Tasks (How actors perform actions)
**Location**: `UI/Screenplay/Tasks/`

Complex multi-step actions:
```csharp
public class LoginToSauceDemo : ITask
{
    private readonly string _username;
    private readonly string _password;

    public async Task PerformAs(Actor actor)
    {
        var page = actor.Using<BrowseTheWeb>().Page;
        var loginPage = new SauceDemoLoginPage(page);
        await loginPage.LoginAsync(_username, _password);
    }
}
```

#### Questions (What actors observe) ✨ NEW
**Location**: `UI/Screenplay/Questions/` + `API/Questions/`

Data retrieval for assertions (read-only operations):

**UI Questions**:
```csharp
// Get text content
var text = await actor.Asks(TheText.Of(".product-label"));

// Check visibility
var isVisible = await actor.Asks(TheVisibility.Of("#checkout-button"));

// Count elements
var count = await actor.Asks(TheCount.Of(".inventory-item"));

// Get value
var value = await actor.Asks(TheValue.Of("input[name='quantity']"));

// Get todo items (domain-specific)
var items = await actor.Asks(TheTodoItems.All());
```

**API Questions**:
```csharp
// Get response status
var status = await actor.Asks(TheResponseStatus.Code);

// Get response body
var pokemon = await actor.Asks(TheResponseBody<Pokemon>.Deserialize());
```

**Key Principles**:
- Questions are **read-only** (never modify state)
- Named with **The{Property}** pattern
- Return data for **assertions**
- Implement `IQuestion<T>` interface
- Used with `Actor.Asks()` method

**Example in Then steps**:
```csharp
[Then(@"the list should contain (.*) items")]
public async Task ThenTheListShouldContainItems(int expectedCount)
{
    await ExecuteThenAsync(async () =>
    {
        // ✅ Use Questions pattern
        var actualCount = await Actor!.Asks(TheCount.Of(Locators.TodoItem));
        Assert.That(actualCount, Is.EqualTo(expectedCount));
    });
}
```

## Technology Stack

| Layer | Technology | Version | Purpose |
|-------|-----------|---------|---------|
| Language | C# | 12.0 | Main language |
| Runtime | .NET | 8.0 | Runtime environment |
| BDD | Reqnroll | 3.3.0 | Gherkin support (SpecFlow successor) |
| Test Framework | NUnit | 3.14.0 | Test execution |
| UI/API Automation | Playwright | 1.57.0 | Browser/API automation |
| DI Container | Autofac | 8.2.0 | Dependency injection |
| Reporting | Allure | 2.14.1 | Test reporting |
| Test Data | Bogus | 35.6.5 | Fake data generation |

## Configuration Management

### Multi-Environment Support
```
appsettings.json                    (Base configuration)
appsettings.Development.json        (Dev overrides)
appsettings.Production.json         (Prod overrides)
Environment Variables               (Runtime overrides)
```

### Priority Order
1. Environment Variables (highest)
2. `appsettings.{DOTNET_ENVIRONMENT}.json`
3. `appsettings.json` (lowest)

### Secret Management
- ❌ Never commit secrets to `appsettings.Development.json`
- ✅ Use GitHub Secrets in CI/CD
- ✅ Use environment variables locally

## Test Execution Flow

### API Test Flow
```
Feature File
    ↓
Given Step → Service Layer → API Client → Playwright APIRequestContext
    ↓
When Step → Validate request
    ↓
Then Step → Assert response + Attach to Allure
```

### UI Test Flow
```
Feature File
    ↓
Given Step → Initialize Actor + Page
    ↓
When Step → Screenplay Task → Page Object → Playwright Browser
    ↓
Then Step → Questions (Actor.Asks) → Assert response + Capture screenshots
```

**Example Flow**:
```
Given the user navigates to TodoMVC
  → Actor initialized with BrowseTheWeb ability
  → NavigateToPlaywrightDemo task executed

When the user adds "Buy milk" to the list
  → AddTodoItem task executed
  → Page object methods called
  → Playwright automation

Then the list should contain 1 item
  → TheCount.Of(Locators.TodoItem) question asked
  → Actor.Asks() retrieves count
  → Assert.That(count, Is.EqualTo(1))
  → Screenshots captured automatically
```

## Reporting Architecture

### Screenshot Configuration
Configurable via `appsettings.json`:
```json
"ScreenshotOptions": {
  "BeforeStep": false,   // Capture before each step
  "AfterStep": false,    // Capture after each step
  "OnFailure": true      // Capture on failure
}
```

### Video Recording
```json
"VideoEnabled": true  // Records full scenario execution
```

### Allure Attachments
- Request/Response details (API)
- Screenshots (UI)
- Videos (UI)
- Logs
- Test data

## CI/CD Integration

### GitHub Actions Workflows
1. **ci-cd.yml**: PR validation
   - API tests (always)
   - UI tests (on demand)
   - Smoke tests

2. **nightly-tests.yml**: Full suite
   - All browsers
   - Email notifications
   - Long retention

### Docker Support
```bash
# API tests only
docker-compose up api-tests

# UI tests (multi-browser)
docker-compose up ui-tests-chromium ui-tests-firefox

# All tests
docker-compose up
```

## Extensibility

### Adding New API Client
1. Create client in `API/Clients/`
2. Create service interface in `Core/Services/IApiServices.cs`
3. Implement service in `Core/Services/ApiServices.cs`
4. Register in `DependencyInjectionConfig.cs`
5. Inject in step bindings

### Adding New Question
1. Create question class in `UI/Screenplay/Questions/` or `API/Questions/`
2. Implement `IQuestion<T>` interface
3. Follow `The{Property}` naming convention
4. Use fluent factory methods (Of, With, etc.)
5. Keep it read-only (never modify state)
6. Use in Then steps with `Actor.Asks()`

Example:
```csharp
// Create question
public class TheCssClass : IQuestion<string>
{
    private readonly string _selector;

    private TheCssClass(string selector) => _selector = selector;

    public static TheCssClass Of(string selector) => new(selector);

    public async Task<string> AnsweredBy(Actor actor)
    {
        return await actor.Page.GetAttributeAsync(_selector, "class") ?? "";
    }
}

// Use in step binding
[Then(@"the element should have class ""(.*)""")]
public async Task ThenElementShouldHaveClass(string expectedClass)
{
    await ExecuteThenAsync(async () =>
    {
        var actualClass = await Actor!.Asks(TheCssClass.Of(".element"));
        Assert.That(actualClass, Does.Contain(expectedClass));
    });
}
```

### Adding New Environment
1. Create `appsettings.{EnvironmentName}.json`
2. Set `DOTNET_ENVIRONMENT={EnvironmentName}`
3. Override specific settings

## Best Practices

### DO ✅
- Use dependency injection
- Use type-safe contexts
- Capture evidence on failure
- Use async/await consistently
- Follow naming conventions
- Write descriptive Gherkin scenarios
- Use centralized timeouts
- Implement retry logic for flaky operations

### DON'T ❌
- Hardcode credentials
- Use magic strings
- Put business logic in step definitions
- Ignore test failures
- Skip code reviews
- Commit sensitive data
- Use `Thread.Sleep()`
- Ignore warnings

## Performance Considerations

### Parallel Execution
Currently disabled to prevent Playwright context conflicts:
```xml
<ParallelizeTestCollections>false</ParallelizeTestCollections>
```

Future: Enable with proper scope isolation using `[Scope]` attributes.

### Resource Management
- Browsers disposed after each scenario
- API contexts reused within scenario
- Allure lifecycle managed automatically

## Troubleshooting

See [RUNBOOK.md](RUNBOOK.md) for common issues and solutions.
