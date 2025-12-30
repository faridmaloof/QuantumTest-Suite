# Contributing Guide - Quantum Test Suite

## Code of Conduct

Be professional, respectful, and collaborative. This framework is built for team success.

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022+ or VS Code
- Git
- Node.js (for Playwright browsers)

### Initial Setup
```powershell
# Clone repository
git clone <repository-url>
cd "QuantumTest Suite"

# Restore dependencies
dotnet restore

# Install Playwright browsers
pwsh -c "npx playwright install --with-deps chromium"

# Copy environment file
copy .env.example .env

# Set development environment
$env:DOTNET_ENVIRONMENT="Development"

# Run tests
dotnet test --filter "Category=api"
```

## Branch Strategy

### Main Branches
- `main`: Production-ready code
- `develop`: Integration branch
- `feature/*`: Feature development
- `bugfix/*`: Bug fixes
- `hotfix/*`: Production hotfixes

### Branch Naming
```
feature/add-new-api-client
bugfix/fix-screenshot-capture
hotfix/critical-login-issue
```

## Coding Standards

### C# Conventions

#### Naming
```csharp
// ✅ Correct
public class BookingService : IBookingService
{
    private readonly IPlaywright _playwright;
    public async Task<ApiResponse<BookingResponse>> CreateBookingAsync(BookingRequest request)
    {
        var client = new RestfulBookerClient(_context);
        return await client.CreateBookingAsync(request);
    }
}

// ❌ Incorrect
public class bookingservice  // Wrong casing
{
    private IPlaywright playwright;  // Missing underscore prefix
    public async Task<ApiResponse<BookingResponse>> create_booking(BookingRequest request)  // Wrong naming
    {
        var Client = new RestfulBookerClient(_context);  // Wrong casing
    }
}
```

#### File Organization
```
- One class per file
- File name matches class name
- Organize using statements (System first, then third-party, then local)
```

#### Async/Await
```csharp
// ✅ Always use async/await for I/O operations
public async Task<bool> IsVisibleAsync()
{
    return await _page.IsVisibleAsync(Locator);
}

// ❌ Don't block async calls
public bool IsVisible()
{
    return _page.IsVisibleAsync(Locator).Result;  // Deadlock risk!
}
```

### Gherkin Conventions

#### Feature Files
```gherkin
# ✅ Good: Clear, business-focused, language-agnostic
Feature: User Authentication
  As a user
  I want to log in to the system
  So that I can access my account

  @smoke @api
  Scenario: Successful login with valid credentials
    Given the user has valid credentials
    When the user attempts to log in
    Then the login should succeed
    And the user should see their dashboard

# ❌ Bad: Implementation details, technical language
Feature: Login API endpoint testing
  Scenario: POST /auth/login returns 200
    Given I send a POST to "https://api.com/auth/login"
    With JSON body {"user": "test", "pass": "123"}
    Then status code is 200
```

#### File Naming
```
✅ UserAuthentication.feature
✅ BookingManagement.feature
❌ user_authentication.feature  (use PascalCase, not snake_case)
❌ test_login.feature           (avoid "test" prefix)
```

### Step Definition Conventions

#### Structure
```csharp
[Binding]
public class AuthenticationStepBindings
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IAuthService _authService;
    private readonly AppSettings _settings;
    
    // Constructor injection
    public AuthenticationStepBindings(
        ScenarioContext scenarioContext,
        IAuthService authService,
        AppSettings settings)
    {
        _scenarioContext = scenarioContext;
        _authService = authService;
        _settings = settings;
    }
    
    // Use type-safe context
    private ApiTestContext Context => _scenarioContext.Get<ApiTestContext>();
    
    // Descriptive step methods
    [Given(@"the user has valid credentials")]
    public void GivenTheUserHasValidCredentials()
    {
        Context.Username = _settings.Users.SauceDemo.Username;
        Context.Password = _settings.Users.SauceDemo.Password;
    }
}
```

#### Step Definition Rules
- Keep steps thin (delegate to services)
- One logical action per step
- Use descriptive method names
- Inject dependencies via constructor
- Use type-safe contexts

## Testing Guidelines

### Test Structure (AAA Pattern)
```csharp
[Test]
public async Task ConfigManager_Should_LoadSettings_Successfully()
{
    // Arrange
    Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
    
    // Act
    var settings = ConfigManager.Settings;
    
    // Assert
    Assert.That(settings, Is.Not.Null);
    Assert.That(settings.Env, Is.EqualTo("development"));
}
```

### Test Data
```csharp
// ✅ Use Factories for dynamic data
var booking = BookingFactory.Create();

// ✅ Use appsettings for configuration
var url = _settings.Apis.RestfulBooker;

// ❌ Don't hardcode test data
var booking = new BookingRequest 
{ 
    Firstname = "John",  // Avoid hardcoding
    Lastname = "Doe" 
};
```

### Assertions
```csharp
// ✅ Descriptive assertion messages
Assert.That(response.StatusCode, Is.EqualTo(200), 
    $"Expected 200 but got {response.StatusCode}");

// ✅ Multiple focused assertions
Assert.That(booking.Firstname, Is.Not.Empty, "Firstname should not be empty");
Assert.That(booking.Bookingid, Is.GreaterThan(0), "Booking ID should be positive");

// ❌ Generic assertions without context
Assert.That(response.StatusCode, Is.EqualTo(200));
```

## Pull Request Process

### Before Creating PR

1. **Update from develop**
   ```bash
   git checkout develop
   git pull origin develop
   git checkout feature/your-feature
   git rebase develop
   ```

2. **Run tests locally**
   ```bash
   dotnet test
   ```

3. **Check code quality**
   ```bash
   dotnet format --verify-no-changes
   ```

### PR Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing performed

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] No new warnings generated
- [ ] Tests pass locally
```

### PR Review Criteria

Reviewers will check:
- ✅ Code follows conventions
- ✅ Tests are comprehensive
- ✅ No hardcoded secrets
- ✅ Error handling is appropriate
- ✅ Documentation is updated
- ✅ No unnecessary complexity

## Adding New Features

### Adding New API Client

1. **Create model** in `API/Models/`
   ```csharp
   public class UserResponse
   {
       public int Id { get; set; }
       public string Name { get; set; } = string.Empty;
   }
   ```

2. **Create client** in `API/Clients/`
   ```csharp
   public class UserClient
   {
       private readonly IAPIRequestContext _context;
       
       public async Task<ApiResponse<UserResponse>> GetUserAsync(int id)
       {
           var response = await _context.GetAsync($"/users/{id}");
           return await ApiResponseMapper.FromAsync<UserResponse>(response);
       }
   }
   ```

3. **Create service interface** in `Core/Services/IApiServices.cs`
   ```csharp
   public interface IUserService
   {
       Task<ApiResponse<UserResponse>> GetUserAsync(int id);
   }
   ```

4. **Implement service** in `Core/Services/ApiServices.cs`
   ```csharp
   public class UserService : IUserService
   {
       // Implementation
   }
   ```

5. **Register in DI** (`Core/DependencyInjection/DependencyInjectionConfig.cs`)
   ```csharp
   builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
   ```

6. **Use in step bindings**
   ```csharp
   public class UserStepBindings
   {
       private readonly IUserService _userService;
       
       public UserStepBindings(IUserService userService)
       {
           _userService = userService;
       }
   }
   ```

### Adding New Page Object

1. **Create locators** in `UI/Locators/`
   ```csharp
   public static class ProductPageLocators
   {
       public const string ProductTitle = "[data-test='product-title']";
       public const string AddToCartButton = "[data-test='add-to-cart']";
   }
   ```

2. **Create page** in `UI/Pages/`
   ```csharp
   public class ProductPage
   {
       private readonly IPage _page;
       
       public ProductPage(IPage page) => _page = page;
       
       public async Task AddToCartAsync()
       {
           await _page.ClickAsync(ProductPageLocators.AddToCartButton);
       }
   }
   ```

3. **Create screenplay task** (optional) in `UI/Screenplay/Tasks/`
   ```csharp
   public class AddProductToCart : ITask
   {
       private readonly string _productName;
       
       public AddProductToCart(string productName) => _productName = productName;
       
       public async Task ExecuteAsync(Actor actor)
       {
           var page = new ProductPage(actor.Page);
           await page.AddToCartAsync();
       }
   }
   ```

## Documentation Standards

### Code Comments
```csharp
// ✅ Document WHY, not WHAT
// Retry logic needed due to intermittent network timeouts
await RetryHelper.ExecuteAsync(async () => await client.GetAsync());

// ❌ Don't state the obvious
// This method gets a user
public async Task<User> GetUserAsync(int id) { }
```

### XML Documentation
```csharp
/// <summary>
/// Executes an action with retry logic to handle transient failures
/// </summary>
/// <param name="action">The action to execute</param>
/// <param name="maxAttempts">Maximum number of retry attempts (default: 3)</param>
/// <param name="delayMilliseconds">Delay between retries in milliseconds (default: 1000)</param>
/// <returns>Task representing the async operation</returns>
public static async Task ExecuteAsync(
    Func<Task> action,
    int maxAttempts = 3,
    int delayMilliseconds = 1000)
{
    // Implementation
}
```

## Common Pitfalls

### ❌ Don't Do This
```csharp
// Hardcoded waits
Thread.Sleep(5000);

// Ignoring exceptions
try { await action(); } catch { }

// String-based context access
var user = _scenarioContext["user"] as User;

// Synchronous blocking of async
var result = DoSomethingAsync().Result;
```

### ✅ Do This Instead
```csharp
// Use smart waits
await WaitHelper.WaitForConditionAsync(() => element.IsVisibleAsync());

// Log exceptions
try { await action(); } 
catch (Exception ex) 
{ 
    ConsoleLogger.Error($"Failed: {ex.Message}"); 
    throw; 
}

// Type-safe context
var context = _scenarioContext.Get<ApiTestContext>();
var user = context.User;

// Proper async
var result = await DoSomethingAsync();
```

## Questions?

- Create an issue with label `question`
- Ask in team chat
- Review [ARCHITECTURE.md](ARCHITECTURE.md)
- Check [RUNBOOK.md](RUNBOOK.md)
