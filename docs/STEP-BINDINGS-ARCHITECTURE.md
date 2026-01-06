# Step Bindings Architecture

## Overview

This document describes the modular architecture of step bindings in QuantumTestSuite, which leverages inheritance-based automatic evidence capture and follows SOLID principles for maintainability.

## Base Classes Pattern

### UiStepBindingsBase

**Location**: `Tests/StepBindings/Base/UiStepBindingsBase.cs`

Base class for all UI-related step bindings that provides automatic screenshot and video capture.

**Key Features**:
- Automatic screenshot capture on every step (Given/When/Then)
- Automatic video recording of entire test execution
- Before/After step screenshot hooks for assertions
- OnFailure screenshot capture for debugging
- Actor initialization with Playwright/Browser management

**Methods**:

```csharp
// Execute Given steps with automatic screenshot after action
protected async Task ExecuteGivenAsync(Func<Task> action)

// Execute When steps with automatic screenshot after action
protected async Task ExecuteWhenAsync(Func<Task> action)

// Execute Then steps with screenshots before and after assertion
protected async Task ExecuteThenAsync(Func<Task> assertion)

// Helper to initialize Actor, Browser, Context, Page
protected async Task EnsureActorAsync(string actorName)

// Helper for navigation
protected async Task NavigateToAsync(string url)

// Execute Screenplay tasks with automatic screenshot
protected async Task PerformTaskAsync(ITask task)
```

**Usage Example**:

```csharp
[Binding]
public class SauceDemoStepBindings : UiStepBindingsBase
{
    public SauceDemoStepBindings(
        UiTestContext context,
        IPlaywrightDriver playwrightDriver,
        ILoggerFactory loggerFactory)
        : base(context, playwrightDriver, loggerFactory)
    {
    }

    [Given(@"el usuario está en la página de SauceDemo")]
    public async Task GivenElUsuarioEstaEnSauceDemo()
    {
        await ExecuteGivenAsync(async () =>
        {
            await EnsureActorAsync("QA");
            var page = new SauceDemoLoginPage(Context.Page!);
            await page.NavigateAsync();
        });
        // Screenshot captured automatically by ExecuteGivenAsync
    }

    [When(@"ingresa credenciales válidas")]
    public async Task WhenIngresaCredencialesValidas()
    {
        await ExecuteWhenAsync(async () =>
        {
            await PerformTaskAsync(new LoginToSauceDemo("standard_user", "secret_sauce"));
        });
        // Screenshot captured automatically by ExecuteWhenAsync
    }

    [Then(@"debe ver la página de inventario")]
    public async Task ThenDebeVerPaginaInventario()
    {
        await ExecuteThenAsync(async () =>
        {
            // ✅ BEST PRACTICE: Use Questions for assertions
            var isVisible = await Actor!.Asks(TheVisibility.Of("#inventory_container"));
            Assert.That(isVisible, Is.True, "Inventory page should be visible");
        });
        // Screenshots captured before and after assertion automatically
    }
}
```

### ApiStepBindingsBase

**Location**: `Tests/StepBindings/Base/ApiStepBindingsBase.cs`

Base class for all API-related step bindings that provides automatic request/response logging to Allure reports.

**Key Features**:
- Automatic request logging (method, URL, headers, body as JSON)
- Automatic response logging (status code, headers, body)
- JSON body detection and formatting
- Exception handling with detailed error logging
- Generic and non-generic overloads for flexibility

**Methods**:

```csharp
// Execute API call with automatic logging (generic version)
protected async Task<ApiResponse<T>> ExecuteApiCallAsync<T>(
    string method,
    string url,
    Func<Task<ApiResponse<T>>> apiCall,
    object? requestBody = null,
    Dictionary<string, string>? headers = null)

// Execute API call with automatic logging (non-generic overload)
protected async Task<ApiResponse<object>> ExecuteApiCallAsync(
    string method,
    string url,
    Func<Task<ApiResponse<object>>> apiCall,
    object? requestBody = null,
    Dictionary<string, string>? headers = null)

// Helper to format headers for display
protected string FormatHeaders(IDictionary<string, string>? headers)
```

**Usage Example**:

```csharp
[Binding]
public class RestfulBookerStepBindings : ApiStepBindingsBase
{
    private readonly IBookingService _bookingService;

    public RestfulBookerStepBindings(
        ApiTestContext context,
        IBookingService bookingService,
        ILoggerFactory loggerFactory)
        : base(context, loggerFactory)
    {
        _bookingService = bookingService;
    }

    [Given(@"un payload válido de booking")]
    public void GivenUnPayloadValidoDeBooking()
    {
        Context.BookingRequest = new BookingRequest
        {
            Firstname = "John",
            Lastname = "Doe",
            Totalprice = 100,
            Depositpaid = true,
            Bookingdates = new BookingDates
            {
                Checkin = "2024-01-01",
                Checkout = "2024-01-05"
            },
            Additionalneeds = "Breakfast"
        };
    }

    [When(@"se envía POST \/booking")]
    public async Task WhenSeEnviaPostBooking()
    {
        var booking = Context.BookingRequest!;
        
        // Request and response automatically logged to Allure
        var response = await ExecuteApiCallAsync(
            "POST",
            $"{Settings.Apis.RestfulBooker}/booking",
            () => _bookingService.CreateBookingAsync(booking),
            requestBody: booking);

        Context.BookingResponse = response;
    }

    [Then(@"el response contiene bookingid")]
    public void ThenElResponseContieneBookingid()
    {
        var response = Context.BookingResponse!;
        var bookingId = response.Body?.GetProperty("bookingid").GetInt32();
        
        Assert.That(bookingId, Is.GreaterThan(0),
            "Booking ID should be greater than 0");
    }
}
```

## Modular Organization

### Folder Structure

```
Tests/StepBindings/
├── Base/                                   # Base classes with automatic evidence capture
│   ├── ApiStepBindingsBase.cs             # Automatic API logging
│   └── UiStepBindingsBase.cs              # Automatic screenshot/video
├── Api/                                    # API-specific step bindings
│   ├── HttpBinStepBindings.cs             # HttpBin API tests
│   ├── RestfulBookerStepBindings.cs       # Restful Booker CRUD tests
│   ├── GitHubStepBindings.cs              # GitHub API tests
│   └── ApiCommonStepBindings.cs           # Common API assertions
├── Ui/                                     # UI-specific step bindings
│   ├── SauceDemoStepBindings.cs           # SauceDemo login tests
│   ├── UltimateQaStepBindings.cs          # UltimateQA form tests
│   └── GitHubUiStepBindings.cs            # GitHub UI + API E2E tests
└── UnitFeatures/                           # Unit test step bindings
    ├── AbilitiesTestsStepBindings.cs
    ├── ConfigManagerStepBindings.cs
    ├── TestDataFactoryStepBindings.cs
    └── UtilitiesStepBindings.cs
```

### Design Principles

1. **Single Responsibility**: Each step binding file handles one specific feature or API
2. **DRY (Don't Repeat Yourself)**: Common logic in base classes, no manual screenshot/logging calls
3. **Open/Closed**: Easy to extend by creating new step binding files without modifying existing code
4. **Maintainability**: Small files (~50-75 lines) are easier to understand and modify
5. **Automatic Registration**: Assembly scanning automatically registers new step bindings

### File Size Guidelines

- **Base Classes**: ~100-150 lines (complex, reusable logic)
- **API Step Bindings**: ~40-80 lines per file (focused on single API/endpoint)
- **UI Step Bindings**: ~50-70 lines per file (focused on single page/flow)
- **Common Step Bindings**: ~40-60 lines (shared assertions/helpers)

## Dependency Injection

### Assembly Scanning Configuration

**Location**: `Tests/Core/DependencyInjection/DependencyInjectionConfig.cs`

Step bindings are automatically registered using Autofac assembly scanning:

```csharp
// Auto-register all API step bindings
builder.RegisterAssemblyTypes(typeof(DependencyInjectionConfig).Assembly)
    .Where(t => t.Namespace != null && t.Namespace.Contains("StepBindings.Api"))
    .InstancePerLifetimeScope();

// Auto-register all UI step bindings
builder.RegisterAssemblyTypes(typeof(DependencyInjectionConfig).Assembly)
    .Where(t => t.Namespace != null && t.Namespace.Contains("StepBindings.Ui"))
    .InstancePerLifetimeScope();
```

**Benefits**:
- No need to modify DI config when adding new step bindings
- Consistent lifecycle management (InstancePerLifetimeScope)
- Follows convention over configuration

## Evidence Capture Details

### Screenshot Strategy (UI Tests)

| Step Type | Before Screenshot | After Screenshot | On Failure |
|-----------|------------------|------------------|------------|
| Given     | ❌               | ✅               | ✅         |
| When      | ❌               | ✅               | ✅         |
| Then      | ✅               | ✅               | ✅         |

**File Naming Convention**:
- Given: `after_given_<timestamp>.png`
- When: `after_when_<timestamp>.png`
- Then: `before_then_<timestamp>.png`, `after_then_<timestamp>.png`
- Failure: `on_failure_<timestamp>.png`

### Video Recording (UI Tests)

- **When**: Automatically started when Actor is initialized
- **Format**: WebM (Chromium/Chrome), MP4 fallback
- **Location**: `Tests/Reports/AllureResults/videos/`
- **Attachment**: Automatically attached to Allure report on test completion

### API Logging (API Tests)

**Request Details**:
```json
{
  "method": "POST",
  "url": "https://restful-booker.herokuapp.com/booking",
  "headers": {
    "Content-Type": "application/json",
    "Accept": "application/json"
  },
  "body": {
    "firstname": "John",
    "lastname": "Doe",
    "totalprice": 100,
    "depositpaid": true,
    "bookingdates": {
      "checkin": "2024-01-01",
      "checkout": "2024-01-05"
    }
  }
}
```

**Response Details**:
```json
{
  "statusCode": 200,
  "headers": {
    "Content-Type": "application/json",
    "Server": "Cowboy"
  },
  "body": {
    "bookingid": 123,
    "booking": { ... }
  }
}
```

## Adding New Step Bindings

### For API Tests

1. Create new file in `Tests/StepBindings/Api/`
2. Inherit from `ApiStepBindingsBase`
3. Inject required services via constructor
4. Use `ExecuteApiCallAsync()` for all API calls
5. No need to modify DI config (auto-registered)

**Example**:

```csharp
using QuantumTestSuite.Core.Context;
using QuantumTestSuite.Core.Services;
using QuantumTestSuite.Tests.StepBindings.Base;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace QuantumTestSuite.Tests.StepBindings.Api
{
    [Binding]
    public class NewApiStepBindings : ApiStepBindingsBase
    {
        private readonly INewService _newService;

        public NewApiStepBindings(
            ApiTestContext context,
            INewService newService,
            ILoggerFactory loggerFactory)
            : base(context, loggerFactory)
        {
            _newService = newService;
        }

        [When(@"llamo al nuevo endpoint")]
        public async Task WhenLlamoAlNuevoEndpoint()
        {
            var response = await ExecuteApiCallAsync(
                "GET",
                $"{Settings.Apis.NewApi}/endpoint",
                () => _newService.GetDataAsync());

            Context.LastResponse = response.Body;
        }
    }
}
```

### For UI Tests

1. Create new file in `Tests/StepBindings/Ui/`
2. Inherit from `UiStepBindingsBase`
3. Inject `UiTestContext` and `IPlaywrightDriver` via constructor
4. Use `ExecuteGivenAsync()`, `ExecuteWhenAsync()`, `ExecuteThenAsync()`
5. No need to modify DI config (auto-registered)

**Example**:

```csharp
using QuantumTestSuite.Core.Context;
using QuantumTestSuite.UI.Drivers;
using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.Tests.StepBindings.Base;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Reqnroll;

namespace QuantumTestSuite.Tests.StepBindings.Ui
{
    [Binding]
    public class NewPageStepBindings : UiStepBindingsBase
    {
        public NewPageStepBindings(
            UiTestContext context,
            IPlaywrightDriver playwrightDriver,
            ILoggerFactory loggerFactory)
            : base(context, playwrightDriver, loggerFactory)
        {
        }

        [Given(@"usuario está en nueva página")]
        public async Task GivenUsuarioEstaEnNuevaPagina()
        {
            await ExecuteGivenAsync(async () =>
            {
                await EnsureActorAsync("QA");
                var page = new NewPage(Context.Page!);
                await page.NavigateAsync();
            });
        }

        [When(@"realiza acción")]
        public async Task WhenRealizaAccion()
        {
            await ExecuteWhenAsync(async () =>
            {
                await PerformTaskAsync(new NewTask(...));
            });
        }

        [Then(@"debe ver resultado")]
        public async Task ThenDebeVerResultado()
        {
            await ExecuteThenAsync(async () =>
            {
                var page = new NewPage(Context.Page!);
                var isVisible = await page.IsResultVisibleAsync();
                Assert.That(isVisible, Is.True);
            });
        }
    }
}
```

## Best Practices

### DO ✅

- **Always** inherit from `ApiStepBindingsBase` or `UiStepBindingsBase`
- **Always** use `ExecuteApiCallAsync()` for API calls (automatic logging)
- **Always** use `ExecuteGivenAsync()`, `ExecuteWhenAsync()`, `ExecuteThenAsync()` for UI steps (automatic screenshots)
- Keep step binding files small and focused (one feature/API per file)
- Use meaningful file names that match the feature they test
- Document complex step bindings with XML comments
- Use descriptive variable names in step definitions

### DON'T ❌

- **Don't** call `AllureHelper.AttachScreenshot()` manually in UI tests (automatic)
- **Don't** call `AllureHelper.AttachRequestDetails()` manually in API tests (automatic)
- **Don't** create monolithic step binding files (>200 lines)
- **Don't** duplicate logic across step bindings (extract to base classes or helpers)
- **Don't** skip using base class methods (bypasses automatic evidence capture)
- **Don't** manually register step bindings in DI config (use assembly scanning)

## Troubleshooting

### Step Definitions Not Found

**Problem**: Reqnroll reports "Undefined step: ..." errors

**Solutions**:
1. Rebuild solution: `dotnet build`
2. Check namespace matches pattern: `*.StepBindings.Api` or `*.StepBindings.Ui`
3. Verify class has `[Binding]` attribute
4. Check DI registration in `DependencyInjectionConfig.cs`

### Screenshots Not Captured

**Problem**: No screenshots in Allure report

**Solutions**:
1. Verify step binding inherits from `UiStepBindingsBase`
2. Ensure using `ExecuteGivenAsync/WhenAsync/ThenAsync` methods
3. Check `PlaywrightDriver` is properly initialized
4. Verify Allure results directory exists: `Tests/Reports/AllureResults/`

### API Logs Missing

**Problem**: Request/response not attached to Allure

**Solutions**:
1. Verify step binding inherits from `ApiStepBindingsBase`
2. Ensure using `ExecuteApiCallAsync()` method
3. Check service methods return `ApiResponse<T>` type
4. Verify `AllureHelper` is working (check other tests)

## Migration Guide

### From Monolithic to Modular

If you have existing monolithic step binding files (>200 lines), follow this migration:

1. **Identify logical groupings**: Group steps by feature, API, or page
2. **Create new files**: One file per logical group in appropriate folder (Api/Ui)
3. **Extract steps**: Move related step definitions to new files
4. **Update inheritance**: Ensure each file inherits from correct base class
5. **Test compilation**: `dotnet build` should succeed
6. **Run tests**: Verify all tests still pass
7. **Delete old files**: Remove monolithic files after validation
8. **Update documentation**: Document new structure

**Example Migration**:

Before (monolithic):
```
StepBindings/
└── ApiStepBindings.cs (300 lines: HttpBin + Restful Booker + GitHub)
```

After (modular):
```
StepBindings/Api/
├── HttpBinStepBindings.cs (45 lines)
├── RestfulBookerStepBindings.cs (75 lines)
├── GitHubStepBindings.cs (45 lines)
└── ApiCommonStepBindings.cs (45 lines)
```

## Best Practices for Then Steps ✨

### ✅ DO: Use Questions Pattern for Assertions

Questions represent "what the actor sees" - data retrieval for verification.

**UI Questions Examples**:
```csharp
[Then(@"the list should contain (.*) items")]
public async Task ThenTheListShouldContainItems(int expectedCount)
{
    await ExecuteThenAsync(async () =>
    {
        // ✅ Use Questions pattern
        var actualCount = await Actor!.Asks(TheCount.Of(".todo-item"));
        Assert.That(actualCount, Is.EqualTo(expectedCount));
    });
}

[Then(@"the heading should display ""(.*)""")]
public async Task ThenTheHeadingShouldDisplay(string expectedText)
{
    await ExecuteThenAsync(async () =>
{
        // ✅ Use TheText question
        var actualText = await Actor!.Asks(TheText.Of("h1.title"));
        Assert.That(actualText, Is.EqualTo(expectedText));
    });
}

[Then(@"the button should be visible")]
public async Task ThenTheButtonShouldBeVisible()
{
    await ExecuteThenAsync(async () =>
    {
        // ✅ Use TheVisibility question
        var isVisible = await Actor!.Asks(TheVisibility.Of("#submit-btn"));
        Assert.That(isVisible, Is.True);
    });
}

[Then(@"the list should include ""(.*)""")]
public async Task ThenTheListShouldInclude(string expectedItem)
{
    await ExecuteThenAsync(async () =>
    {
        // ✅ Use domain-specific question
        var items = await Actor!.Asks(TheTodoItems.All());
        Assert.That(items, Does.Contain(expectedItem));
    });
}
```

**API Questions Examples**:
```csharp
[Then(@"the response status should be (.*)")]
public async Task ThenResponseStatusShouldBe(int expectedStatus)
{
    await ExecuteThenAsync(async () =>
    {
        // ✅ Use TheResponseStatus question
        var status = await Actor!.Asks(TheResponseStatus.Code);
        Assert.That(status, Is.EqualTo(expectedStatus));
    });
}

[Then(@"the response should contain pokemon data")]
public async Task ThenResponseShouldContainPokemonData()
{
    await ExecuteThenAsync(async () =>
    {
        // ✅ Use TheResponseBody question
        var pokemon = await Actor!.Asks(TheResponseBody<Pokemon>.Deserialize());
        Assert.That(pokemon, Is.Not.Null);
        Assert.That(pokemon.Name, Is.Not.Empty);
    });
}
```

### ❌ DON'T: Use Direct Playwright/Page Calls in Then Steps

**Anti-Pattern** (Old way):
```csharp
[Then(@"the list should contain (.*) items")]
public async Task ThenTheListShouldContainItems(int expectedCount)
{
    await ExecuteThenAsync(async () =>
    {
        // ❌ Don't use direct Playwright calls
        var count = await Actor!.Page.Locator(".todo-item").CountAsync();
        Assert.That(count, Is.EqualTo(expectedCount));
    });
}

[Then(@"the heading should display ""(.*)""")]
public async Task ThenTheHeadingShouldDisplay(string expectedText)
{
    await ExecuteThenAsync(async () =>
    {
        // ❌ Don't use direct Page Object methods for assertions
        var page = new TodoPage(Actor!.Page);
        var text = await page.GetHeadingTextAsync();
        Assert.That(text, Is.EqualTo(expectedText));
    });
}
```

### Why Questions Pattern?

1. **Separation of Concerns**: Questions = data retrieval, not UI interaction
2. **Reusability**: Share questions across multiple test scenarios
3. **Testability**: Questions are unit-testable independently
4. **Readability**: `Actor.Asks(TheText.Of())` is more expressive
5. **Maintainability**: Centralized element queries

### Available Built-in Questions

**UI Questions** (`Tests/UI/Screenplay/Questions/`):
- `TheText.Of(selector)` - Get text content
- `TheVisibility.Of(selector)` - Check visibility (returns bool)
- `TheCount.Of(selector)` - Count matching elements
- `TheValue.Of(selector)` - Get input value
- `TheTodoItems.All()` - Get all todo items (domain-specific)

**API Questions** (`Tests/API/Questions/`):
- `TheResponseStatus.Code` - Get HTTP status code
- `TheResponseBody<T>.Deserialize()` - Deserialize response body

### Creating Custom Questions

```csharp
// 1. Implement IQuestion<T>
public class TheCssClass : IQuestion<string>
{
    private readonly string _selector;

    private TheCssClass(string selector) => _selector = selector;

    // 2. Fluent factory method
    public static TheCssClass Of(string selector) => new(selector);

    // 3. Answer method (read-only)
    public async Task<string> AnsweredBy(Actor actor)
    {
        return await actor.Page.GetAttributeAsync(_selector, "class") ?? "";
    }
}

// 4. Use in step binding
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

## Performance Considerations

### Screenshot Performance

- Screenshots add ~100-300ms per step
- Videos add ~50-100ms overhead (encoding happens async)
- Use `PLAYWRIGHT_SCREENSHOT_ENABLED=false` to disable in CI (if needed)

### API Logging Performance

- JSON serialization adds ~10-50ms per call (negligible)
- Logging is async and non-blocking
- Use `ALLURE_ENABLED=false` to disable in local dev (if needed)

## Related Documentation

- [ALLURE-QUICKSTART.md](ALLURE-QUICKSTART.md) - Allure reporting setup
- [ABILITIES-GUIDE.md](ABILITIES-GUIDE.md) - Screenplay pattern abilities
- [ARCHITECTURE.md](ARCHITECTURE.md) - Overall solution architecture (includes Questions pattern)
- [FILE-STRUCTURE-GUIDE.md](FILE-STRUCTURE-GUIDE.md) - Complete file structure with Questions templates
- [CONFIGURATION.md](CONFIGURATION.md) - Configuration management

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2024-01 | Initial modular architecture with base classes |
| 2.0.0 | 2025-01 | Added Questions Pattern best practices and examples |
