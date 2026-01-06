# Step Bindings Architecture

## Overview

This document describes the modular architecture of step bindings in QuantumTestSuite, which leverages inheritance-based automatic evidence capture and follows SOLID principles for maintainability.

## Base Classes Pattern

### UiStepBindingsBase

**Location**: `Tests/StepBindings/Base/UiStepBindingsBase.cs`

**Purpose**: Automatic screenshot/video capture for all UI steps

**Key Methods**: `ExecuteGivenAsync()`, `ExecuteWhenAsync()`, `ExecuteThenAsync()`, `EnsureActorAsync()`, `PerformTaskAsync()`

### ApiStepBindingsBase

**Location**: `Tests/StepBindings/Base/ApiStepBindingsBase.cs`

**Purpose**: Automatic request/response logging to Allure

**Key Methods**: `ExecuteApiCallAsync<T>()`, `ExecuteApiCallAsync(object)`, `FormatHeaders()`

📖 **Complete Implementation**: See [FILE-STRUCTURE-GUIDE.md](FILE-STRUCTURE-GUIDE.md) for full code templates.

## Modular Organization

### Folder Structure

```
Tests/StepBindings/StepBindings/
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

**Location**: `Tests/StepBindings/StepBindings/`

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

**Location**: `Tests/Framework/Core/DependencyInjection/DependencyInjectionConfig.cs`

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

**UI Tests**: Auto screenshots (after Given/When, before & after Then, on failure), videos (full scenario)

**API Tests**: Auto request/response JSON with headers, method, URL attached to Allure

📖 **Configuration**: See [ALLURE-QUICKSTART.md](ALLURE-QUICKSTART.md) for screenshot options, video settings, and troubleshooting.

## Adding New Step Bindings

### For API Tests

1. Create in `Tests/StepBindings/Api/`
2. Inherit from `ApiStepBindingsBase`
3. Use `ExecuteApiCallAsync()` for all API calls (automatic logging)
4. Auto-registered via assembly scanning

📖 **Template**: See [FILE-STRUCTURE-GUIDE.md](FILE-STRUCTURE-GUIDE.md) "Code Templates" section.

### For UI Tests

1. Create in `Tests/StepBindings/Ui/`
2. Inherit from `UiStepBindingsBase`
3. Use `ExecuteGivenAsync/WhenAsync/ThenAsync()` (automatic screenshots)
4. Auto-registered via assembly scanning

📖 **Template**: See [FILE-STRUCTURE-GUIDE.md](FILE-STRUCTURE-GUIDE.md) "Code Templates" section.

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

### ✅ DO: Use Questions Pattern

Questions = read-only queries for assertions.

**Examples**:
- UI: `await Actor!.Asks(TheCount.Of(".todo-item"))`
- API: `await Actor!.Asks(TheResponseStatus.Code)`

### ❌ DON'T: Use Direct Playwright Calls

```csharp
// ❌ Anti-pattern
var count = await Actor!.Page.Locator(".todo-item").CountAsync();

// ✅ Correct  
var count = await Actor!.Asks(TheCount.Of(".todo-item"));
```

**Why Questions?** Separation, reusability, testability, readability.

📖 **Complete Guide**: See [ARCHITECTURE.md](ARCHITECTURE.md) "Questions Pattern" for philosophy and examples.

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
