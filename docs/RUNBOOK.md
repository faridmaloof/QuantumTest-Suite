# Runbook - Quantum Test Suite

## Quick Reference

### Initial Setup (First Time)

```powershell
# 1. Copy environment template
copy .env.example .env

# 2. Edit .env with your credentials
notepad .env

# 3. Install dependencies
dotnet restore

# 4. Install browsers
pwsh -c "npx playwright install --with-deps chromium"
```

### Running Tests Locally

```powershell
# Run API tests only
dotnet test --filter "Category=api"

# Run UI tests (requires RUN_UI_TESTS=true in .env)
dotnet test --filter "Category=ui"

# Run specific feature
dotnet test --filter "FullyQualifiedName~GitHubUserSearch"

# Run smoke tests only
dotnet test --filter "Category=smoke"

# Generate Allure report
./scripts/allure-report.ps1
```

### Switch Between Environments

```powershell
# Use development environment (.env.development)
$env:TEST_ENVIRONMENT="development"
dotnet test

# Use QA environment (.env.qa)
$env:TEST_ENVIRONMENT="qa"
dotnet test

# Use production environment (.env.production)
$env:TEST_ENVIRONMENT="production"
dotnet test
```

### Override Specific Values

```powershell
# Override single value for current session
$env:PLAYWRIGHT_HEADLESS="false"
$env:PLAYWRIGHT_VIDEO_ENABLED="true"
dotnet test

# Override for single command
$env:RUN_UI_TESTS="true"; dotnet test --filter "Category=ui"
```

### Environment Variables Reference

| Variable | Purpose | Default | Values |
|----------|---------|---------|---------|
| `TEST_ENVIRONMENT` | Config environment | local | local, development, qa, staging, production |
| `RUN_UI_TESTS` | Enable UI test execution | false | true, false |
| `PLAYWRIGHT_HEADLESS` | Headless browser mode | true | true, false |
| `PLAYWRIGHT_BROWSER` | Browser to use | chromium | chromium, firefox, webkit |
| `PLAYWRIGHT_BASE_URL` | Base URL for tests | (from .env) | Any valid URL |
| `PLAYWRIGHT_SLOWMO` | Delay between actions | 0 | Milliseconds (0-1000) |
| `PLAYWRIGHT_VIDEO_ENABLED` | Record video | false | true, false |
| `SCREENSHOT_BEFORE_STEP` | Screenshot before steps | false | true, false |
| `SCREENSHOT_AFTER_STEP` | Screenshot after steps | false | true, false |
| `SCREENSHOT_ON_FAILURE` | Screenshot on failure | true | true, false |
| `SAUCEDEMO_USERNAME` | SauceDemo username | (from .env) | standard_user |
| `SAUCEDEMO_PASSWORD` | SauceDemo password | (from .env) | secret_sauce |
| `API_HTTPBIN` | HTTPBin API URL | https://httpbin.org | Any valid URL |
| `API_RESTFUL_BOOKER` | RestfulBooker API | https://restful-booker.herokuapp.com | Any valid URL |
| `API_GITHUB` | GitHub API | https://api.github.com | Any valid URL |
| `ALLURE_RESULTS_DIRECTORY` | Allure output dir | Reports/AllureResults | Any valid path |

## Common Issues and Solutions

### Issue 1: Playwright Browsers Not Found

**Error:**
```
Executable doesn't exist at C:\Users\...\ms-playwright\chromium-1097\chrome-win\chrome.exe
```

**Solution:**
```powershell
# Install browsers
pwsh -c "npx playwright install --with-deps chromium"

# Or install all browsers
pwsh -c "npx playwright install --with-deps"
```

---

### Issue 2: UI Tests Are Skipped

**Error:**
```
UI scenarios skipped. Set RUN_UI_TESTS=true to execute UI tests.
```

**Solution:**
```powershell
# Enable UI tests
$env:RUN_UI_TESTS="true"
dotnet test --filter "Category=ui"
```

---

### Issue 3: Secrets Not Found

**Error:**
```
Value cannot be null. (Parameter 'Username')
```

**Solution:**

**Option 1: Use .env file (recommended)**
```powershell
# 1. Copy template
copy .env.example .env

# 2. Edit .env with real credentials
notepad .env

# 3. Run tests
dotnet test
```

**Option 2: Set environment variables**
```powershell
$env:SAUCEDEMO_USERNAME="standard_user"
$env:SAUCEDEMO_PASSWORD="secret_sauce"
dotnet test
```

**Option 3: Use environment-specific .env**
```powershell
# 1. Copy environment template
copy .env.development.example .env.development

# 2. Edit .env.development with real credentials
notepad .env.development

# 3. Run with environment
$env:TEST_ENVIRONMENT="development"
dotnet test
```

---

### Issue 4: Tests Fail with "Context Disposed"

**Error:**
```
Cannot access a disposed object. Object name: 'BrowserContext'
```

**Cause:** Multiple tests accessing same browser context

**Solution:**
- Ensure each scenario creates its own context
- Check hooks are cleaning up properly
- Review parallel execution settings (currently disabled)

---

### Issue 5: Allure Report Not Generated

**Error:**
```
Could not find allure-results directory
```

**Solution:**
```powershell
# Verify results directory exists
Test-Path "Tests/bin/Debug/net8.0/Reports/AllureResults"

# Run tests first to generate results
dotnet test

# Then generate report
allure generate Tests/bin/Debug/net8.0/Reports/AllureResults -o Reports/AllureReport --clean
allure open Reports/AllureReport
```

---

### Issue 6: Video Files Not Attached

**Symptom:** Videos not appearing in Allure report

**Solution:**
1. Enable video recording in .env:
   ```bash
   PLAYWRIGHT_VIDEO_ENABLED=true
   ```

2. Or set environment variable:
   ```powershell
   $env:PLAYWRIGHT_VIDEO_ENABLED="true"
   dotnet test
   ```

3. Verify video directory exists:
   ```powershell
   Test-Path "Tests/bin/Debug/net8.0/Reports/AllureResults/videos"
   ```

4. Check hooks properly close context:
   ```csharp
   // TestHooks.cs should call
   await AllureHelper.AttachVideoAsync(context.Page);
   ```

---

### Issue 7: Screenshot Configuration Not Working

**Symptom:** Screenshots not captured despite configuration

**Solution:**

1. **Verify .env configuration:**
   ```bash
   SCREENSHOT_BEFORE_STEP=false
   SCREENSHOT_AFTER_STEP=false
   SCREENSHOT_ON_FAILURE=true
   ```

2. **Or override with environment variables:**
   ```powershell
   $env:SCREENSHOT_BEFORE_STEP="true"
   $env:SCREENSHOT_AFTER_STEP="true"
   $env:SCREENSHOT_ON_FAILURE="true"
   dotnet test
   ```

3. **Check AllureHelper initialization:**
   ```csharp
   // Should be called in BeforeTestRun
   AllureHelper.Initialize(settings);
   ```

4. **Verify hooks are registered:**
   ```csharp
   [BeforeStep("ui")]
   [AfterStep("ui")]
   [AfterStep] // For failures
   ```

---

### Issue 8: Dependency Injection Fails

**Error:**
```
Unable to resolve service for type 'IBookingService'
```

**Solution:**

1. **Verify service is registered:**
   ```csharp
   // DependencyInjectionConfig.cs
   builder.RegisterType<BookingService>()
       .As<IBookingService>()
       .InstancePerLifetimeScope();
   ```

2. **Check constructor signature:**
   ```csharp
   public ApiStepBindings(
       ScenarioContext scenarioContext,
       IBookingService bookingService)  // ✅ Interface, not implementation
   {
       _bookingService = bookingService;
   }
   ```

3. **Rebuild project:**
   ```powershell
   dotnet clean
   dotnet build
   ```

---

### Issue 9: Docker Container Fails to Start

**Error:**
```
Error response from daemon: Conflict. The container name is already in use
```

**Solution:**
```powershell
# Stop and remove existing containers
docker-compose down

# Rebuild and start
docker-compose up --build

# View logs
docker-compose logs -f api-tests
```

---

### Issue 10: GitHub Actions Workflow Fails

**Common causes:**

1. **Missing secrets:**
   - Go to GitHub Settings → Secrets → Actions
   - Add `SAUCEDEMO_USERNAME` and `SAUCEDEMO_PASSWORD`

2. **Workflow syntax error:**
   ```bash
   # Validate locally with act
   act -l
   ```

3. **Browser installation fails:**
   - Check `pwsh -c "npx playwright install"` step
   - Verify `--with-deps` flag is included

---

## Debugging Tips

### Enable Verbose Logging

```powershell
# Detailed test output
dotnet test --logger "console;verbosity=detailed"

# Playwright debug mode
$env:DEBUG="pw:api"
dotnet test
```

### Inspect Allure Results

```powershell
# View raw JSON results
Get-Content "Tests/bin/Debug/net8.0/Reports/AllureResults/*-result.json" | ConvertFrom-Json
```

### Check Configuration Loading

```csharp
// Add to test
var settings = ConfigManager.Settings;
Console.WriteLine(ConfigManager.ToJson(settings));
```

### Capture Network Traffic (Playwright)

```csharp
page.Request += (_, request) => Console.WriteLine($">> {request.Method} {request.Url}");
page.Response += (_, response) => Console.WriteLine($"<< {response.Status} {response.Url}");
```

## Performance Optimization

### Reduce Test Execution Time

1. **Skip video recording in CI:**
   ```yaml
   # .github/workflows/ci-cd.yml
   env:
     PLAYWRIGHT_VIDEO_ENABLED: false
   ```

2. **Disable screenshots for passing tests:**
   ```bash
   # .env
   SCREENSHOT_ON_FAILURE=true
   SCREENSHOT_BEFORE_STEP=false
   SCREENSHOT_AFTER_STEP=false
   ```

3. **Use headless mode:**
   ```bash
   # .env
   PLAYWRIGHT_HEADLESS=true
   ```

4. **Enable parallel execution:**
   ```xml
   <!-- QuantumTestSuite.Tests.csproj -->
   <PropertyGroup>
     <LevelOfParallelism>4</LevelOfParallelism>
   </PropertyGroup>
   ```

3. **Run API tests first (they're faster):**
   ```bash
   dotnet test --filter "Category=api" && \
   dotnet test --filter "Category=ui"
   ```

### Reduce Flakiness

1. **Use smart waits:**
   ```csharp
   await WaitHelper.WaitForVisibleAsync(page, selector, TestTimeouts.Default);
   ```

2. **Implement retry logic:**
   ```csharp
   await RetryHelper.ExecuteAsync(async () => await action(), maxAttempts: 3);
   ```

3. **Increase timeouts for slow environments:**
   ```bash
   # .env
   PLAYWRIGHT_SLOWMO=100  # Add 100ms delay between actions
   ```

## Maintenance Tasks

### Weekly
- [ ] Review failed test trends in Allure
- [ ] Check for dependency updates
- [ ] Clean old Allure reports

### Monthly
- [ ] Update Playwright browsers
- [ ] Review and refactor flaky tests
- [ ] Update documentation

### Quarterly
- [ ] Upgrade .NET SDK (if applicable)
- [ ] Upgrade major dependencies
- [ ] Performance audit

## Useful Commands

### Clean Build Artifacts
```powershell
dotnet clean
Remove-Item -Recurse -Force Tests/bin, Tests/obj
```

### Update Dependencies
```powershell
dotnet list package --outdated
dotnet add package <PackageName>
```

### Generate Feature File Code-Behind
```powershell
# Reqnroll regenerates feature files automatically during build
dotnet clean
dotnet build
```

### Check Test Discovery
```powershell
dotnet test -t
```

### Export Allure Report
```powershell
allure generate Tests/bin/Debug/net8.0/Reports/AllureResults -o Reports/AllureReport
Compress-Archive -Path Reports/AllureReport -DestinationPath allure-report.zip
```

## Emergency Contacts

### Framework Issues
- Create GitHub Issue with label `bug`
- Include error message, steps to reproduce, environment details

### CI/CD Issues
- Check GitHub Actions logs
- Verify secrets are configured
- Contact DevOps team

### Environment Access
- Verify VPN connection
- Check firewall rules
- Contact infrastructure team

## Additional Resources

- [Architecture Documentation](ARCHITECTURE.md)
- [Contributing Guide](CONTRIBUTING.md)
- [Playwright Documentation](https://playwright.dev/dotnet)
- [SpecFlow Documentation](https://docs.specflow.org/)
- [Allure Documentation](https://docs.qameta.io/allure/)
