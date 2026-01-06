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
**Location**: `Tests/Framework/UI/Screenplay/` + `Tests/Framework/API/Questions/`

**Purpose**: High-level user interactions following Actor-Ability-Task-Question model

**4 Layers**: Actors (Who), Abilities (What), Tasks (How), Questions (See)

**Key Principle**: Questions are read-only queries for assertions, named with `The{Property}` pattern, used with `Actor.Asks()`.

📖 **Complete Templates & Examples**: See [FILE-STRUCTURE-GUIDE.md](FILE-STRUCTURE-GUIDE.md) for all code templates and detailed examples.

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

**API Tests**: Feature → Step Bindings → Service Layer → API Client → Playwright APIRequestContext → Allure

**UI Tests**: Feature → Step Bindings → Screenplay (Actor/Tasks/Questions) → Page Objects → Playwright Browser → Allure

**Key**: Step Bindings are thin glue layer. Business logic in Services (API) or Tasks (UI). Assertions use Questions pattern.

## Reporting Architecture

**Evidence Capture**: Screenshots (configurable timing), videos (full scenario), request/response JSON (API), logs.

**Allure Attachments**: Automatic attachment via base classes (UiStepBindingsBase, ApiStepBindingsBase).

📖 **Configuration Guide**: See [ALLURE-QUICKSTART.md](ALLURE-QUICKSTART.md) for screenshot options, video settings, and best practices.

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

**Guidelines**:
1. Location: `Tests/Framework/UI/Screenplay/Questions/`
2. Naming: `The{Property}` pattern (e.g., TheCssClass, TheValue)
3. Interface: Implement `IQuestion<T>` with `AnsweredBy(Actor actor)`
4. Read-only: Never modify state (queries only)
5. Fluent: Use factory methods (Of, With, From, etc.)

**Philosophy**: Questions separate data retrieval from assertions, making tests more readable, maintainable, and testable.

📖 **Complete Guide**: See [FILE-STRUCTURE-GUIDE.md](FILE-STRUCTURE-GUIDE.md) for all Question templates and code examples.

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
