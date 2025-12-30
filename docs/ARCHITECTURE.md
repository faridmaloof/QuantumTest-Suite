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
│  SERVICE LAYER   │                      │   SCREENPLAY/POM     │
│  (API Services)  │                      │  (UI Interactions)   │
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

### Screenplay Pattern
**Location**: `UI/Screenplay/`

**Purpose**: High-level user interactions
```csharp
await actor.AttemptsTo(new LoginToSauceDemo(username, password));
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
Then Step → Assert state + Capture screenshots
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

### Adding New Page
1. Create locators in `UI/Locators/`
2. Create page class in `UI/Pages/`
3. Create screenplay task (if needed) in `UI/Screenplay/Tasks/`
4. Use in step bindings

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
