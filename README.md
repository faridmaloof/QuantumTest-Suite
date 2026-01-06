# 🚀 Quantum Test Suite

Enterprise-grade test automation framework built on **.NET 8**, implementing **BDD with Reqnroll**, **UI/API testing with Playwright**, and **comprehensive reporting with Allure**.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Playwright](https://img.shields.io/badge/Playwright-1.57.0-2EAD33?logo=playwright)](https://playwright.dev/dotnet/)
[![Reqnroll](https://img.shields.io/badge/Reqnroll-3.3.0-3CB371)](https://reqnroll.net/)
[![Allure](https://img.shields.io/badge/Allure-2.14.1-FF6C37?logo=allure)](https://docs.qameta.io/allure/)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com)
[![Test Coverage](https://img.shields.io/badge/tests-14%2F14%20passing-brightgreen)](https://github.com)

---

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Prerequisites](#-prerequisites)
- [Quick Start](#-quick-start)
- [Running Tests](#-running-tests)
- [Configuration](#-configuration)
- [Reporting](#-reporting)
- [CI/CD](#-cicd)
- [Documentation](#-documentation)
- [Examples](#-examples)
- [Contributing](#-contributing)

---

## ✨ Features

### Core Capabilities
✅ **BDD with Gherkin** - Business-readable test scenarios using Reqnroll  
✅ **UI Automation** - Cross-browser testing with Playwright  
✅ **API Testing** - REST API validation with Playwright APIRequestContext  
✅ **Unit Tests** - Framework components validation with NUnit + Allure  
✅ **Screenplay Pattern** - 4-layer Actor-Ability-Task-Question model ✨  
✅ **Questions Pattern** - Read-only queries for clean assertions ✨  
✅ **Page Object Model** - Maintainable UI element abstraction  
✅ **Dependency Injection** - Autofac with automatic registration  
✅ **Rich Reporting** - Allure with screenshots, videos, and trends  
✅ **Multi-Environment** - Dev/QA/Staging/Production configs  
✅ **Docker Support** - Reproducible execution environments  
✅ **GitHub Actions** - Ready-to-use CI/CD pipelines  

**Status**: 14/14 passing (100%) - API (5) | UI (3) | Unit (6)

---

## 🏗️ Architecture

### High-Level Overview

```
┌─────────────────────────────────────────────────┐
│           GHERKIN FEATURES (BDD)                │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│       STEP DEFINITIONS (Glue Layer)             │
│  - Modular: Api/, Ui/, Unit/                    │
│  - Base Classes: Auto Evidence Capture          │
└────────┬─────────────────────────┬──────────────┘
         │                          │
         ▼                          ▼
┌────────────────┐         ┌────────────────────────────┐
│  SERVICE LAYER │         │  SCREENPLAY PATTERN (4L) ✨│
│  (API Logic)   │         ├────────────────────────────┤
│                │         │ • Actors (Who)             │
│                │         │ • Abilities (What)         │
│                │         │ • Tasks (How)              │
│                │         │ • Questions (See) ← NEW    │
└────────┬───────┘         └────────────┬───────────────┘
         │                               │
         ▼                               ▼
┌────────────────┐         ┌─────────────────────────────┐
│  API CLIENTS   │         │    PAGES + LOCATORS         │
└────────┬───────┘         └─────────────┬───────────────┘
         │                               │
         └──────────┬────────────────────┘
                    ▼
         ┌──────────────────┐
         │   PLAYWRIGHT      │
         │  (Browser/API)    │
         └──────────┬────────┘
                    ▼
         ┌──────────────────┐
         │   ALLURE          │
         │  (Reporting)      │
         └───────────────────┘
```

### Screenplay Pattern (4-Layer Model) ✨

Framework implements Actor-Ability-Task-Question model: **Actors** perform **Tasks** using **Abilities**, then verify results with **Questions**.

📖 **See [ARCHITECTURE.md](docs/ARCHITECTURE.md) for detailed philosophy and design decisions.**

### Step Bindings Modular Structure

```
Tests/StepBindings/
├── Base/ → Automatic evidence capture
├── Api/ → API test step bindings
├── Ui/ → UI test step bindings  
└── Unit/ → Framework unit tests
```

📖 **Complete Guide**: [STEP-BINDINGS-ARCHITECTURE.md](docs/STEP-BINDINGS-ARCHITECTURE.md)

---

## 📦 Prerequisites

| Requirement | Version | Installation |
|-------------|---------|--------------|
| .NET SDK | 8.0+ | [Download](https://dotnet.microsoft.com/download) |
| Node.js | 20.x+ | [Download](https://nodejs.org/) |
| Git | Latest | [Download](https://git-scm.com/) |
| Visual Studio | 2022+ | [Download](https://visualstudio.microsoft.com/) |
| Allure CLI | Latest | `winget install QA.Allure` |

---

## 🚀 Quick Start

### 1. Clone and Setup

```powershell
# Clone repository
git clone <repository-url>
cd "QuantumTest Suite"

# Restore dependencies
dotnet restore

# Install Playwright browsers
pwsh -c "npx playwright install --with-deps chromium"
```

### 2. Configure Environment

**🔧 Múltiples opciones disponibles** (elige la que prefieras):

**Opción A: Archivo .env (más simple)**
```powershell
copy .env.example .env
notepad .env  # Editar credenciales
```

**Opción B: User Secrets (más seguro)**
```powershell
cd Tests
dotnet user-secrets set "Users:SauceDemo:Username" "standard_user"
dotnet user-secrets set "Users:SauceDemo:Password" "secret_sauce"
```

**Opción C: Variables de entorno (override)**
```powershell
$env:SAUCEDEMO_USERNAME="standard_user"
$env:SAUCEDEMO_PASSWORD="secret_sauce"
```

**📖 Ver [CONFIGURATION.md](docs/CONFIGURATION.md) para guía completa de todas las opciones.**

### 3. Run Tests

```powershell
# API tests only (fast)
dotnet test --filter "Category=api"

# UI tests (requires browser)
$env:RUN_UI_TESTS="true"
dotnet test --filter "Category=ui"

# All tests
dotnet test
```

### 4. View Report

```powershell
# Generate and open Allure report
./scripts/allure-report.ps1 -Open
```

---

## 🧪 Running Tests

### By Test Type

```powershell
# API tests only (fast)
dotnet test --filter "Category=api"

# UI tests (requires browser and RUN_UI_TESTS=true)
$env:RUN_UI_TESTS="true"
dotnet test --filter "Category=ui"

# Unit tests (framework validation)
dotnet test --filter "Category=unit"

# All tests
dotnet test
```

### By Priority

```powershell
# Smoke tests only
dotnet test --filter "Category=smoke"

# Regression suite
dotnet test --filter "Category=regression"

# Specific feature
dotnet test --filter "FullyQualifiedName~GitHubUserSearch"
```

### Environment Variables

| Variable | Purpose | Default | Example |
|----------|---------|---------|---------|
| `TEST_ENVIRONMENT` | Config environment | local | development, qa, staging, production |
| `RUN_UI_TESTS` | Enable UI scenarios | false | true |
| `PLAYWRIGHT_HEADLESS` | Browser mode | true | false (visible) |
| `PLAYWRIGHT_BROWSER` | Browser type | chromium | firefox, webkit |
| `PLAYWRIGHT_VIDEO_ENABLED` | Record video | false | true |
| `SCREENSHOT_BEFORE_STEP` | Before step screenshot | false | true |
| `SCREENSHOT_AFTER_STEP` | After step screenshot | false | true |
| `SCREENSHOT_ON_FAILURE` | Capture on error | true | false |
| `SAUCEDEMO_USERNAME` | Login credential | (from .env) | standard_user |
| `SAUCEDEMO_PASSWORD` | Login password | (from .env) | secret_sauce |

### Docker Execution

```powershell
# Build image
docker-compose build

# Run API tests
docker-compose up api-tests

# Run UI tests (multi-browser)
docker-compose up ui-tests-chromium ui-tests-firefox

# View live Allure report
docker-compose up allure-report
# Open http://localhost:5050
```

---

## ⚙️ Configuration

### 🔧 Multiple Configuration Options Supported

El framework soporta **múltiples opciones de configuración** que pueden coexistir:

✅ **Variables de Entorno** (máxima prioridad) - CI/CD, overrides temporales  
✅ **User Secrets (.NET)** - Desarrollo local seguro  
✅ **.env.{environment}** - Configuración por entorno (dev/qa/prod)  
✅ **.env** - Configuración base local  
✅ **appsettings.json** - Valores por defecto

**📖 Guía Completa:** Ver [CONFIGURATION.md](docs/CONFIGURATION.md) para:
- Ejemplos detallados de cada opción
- Comandos prácticos para cada herramienta CI/CD
- Configuración de User Secrets paso a paso
- Orden de prioridad y resolución de problemas

### Quick Reference

```powershell
# Opción 1: Archivo .env (más simple)
copy .env.example .env
notepad .env

# Opción 2: User Secrets (más seguro para desarrollo)
cd Tests
dotnet user-secrets set "Users:SauceDemo:Username" "standard_user"
dotnet user-secrets set "Users:SauceDemo:Password" "secret_sauce"

# Opción 3: Variables de entorno (overrides temporales)
$env:PLAYWRIGHT_HEADLESS="false"
$env:RUN_UI_TESTS="true"
```

### Configuration Priority

```
1. Environment Variables (highest)
2. User Secrets
3. .env.{TEST_ENVIRONMENT}
4. .env
5. appsettings.json (defaults)
```

---
## 📊 Reporting

### Allure Features

- **Timeline**: Execution flow visualization
- **Trends**: Historical pass/fail rates
- **Categories**: Product bugs vs infrastructure issues
- **Attachments**: Screenshots, videos, JSON, logs
- **Behaviors**: BDD stories organization
- **Suites**: Test grouping by feature

**📖 Guía Completa:** Ver [ALLURE-QUICKSTART.md](docs/ALLURE-QUICKSTART.md) para:
- Generación de reportes (local y CI/CD)
- Configuración de evidencias (screenshots, videos)
- Buenas prácticas y troubleshooting
- Integración con GitHub Actions, Azure DevOps, Jenkins

### Evidence Attached Automatically

**API Tests:**
- Request details (method, URL, headers, body)
- Response details (status, headers, body)

**UI Tests:**
- Screenshots (configurable timing)
- Videos (optional, full scenario)
- Console logs

**Unit Tests:**
- Stack traces
- Assertion details
- Custom attachments

### Generate Report

```powershell
# Local generation (recommended)
./scripts/allure-report.ps1 -Open

# Or manual commands
dotnet test
allure generate Tests/bin/Debug/net8.0/Reports/AllureResults -o Reports/AllureReport --clean
allure open Reports/AllureReport

# Docker option
docker-compose up allure-report
# Navigate to http://localhost:5050
```

---

## 🔄 CI/CD

### GitHub Actions Workflows

**1. `ci-cd.yml`** - Pull Request Validation
- API tests (always)
- UI tests (on-demand or with `[run-ui]` commit message)
- Smoke tests
- Allure report artifact upload

**2. `nightly-tests.yml`** - Full Suite
- Runs at 2 AM UTC daily
- All browsers (Chromium, Firefox)
- Email notifications on failure
- 90-day artifact retention

### Setup Secrets

In GitHub Settings → Secrets → Actions:
```
SAUCEDEMO_USERNAME = standard_user
SAUCEDEMO_PASSWORD = secret_sauce
SMTP_USERNAME = (for notifications)
SMTP_PASSWORD = (for notifications)
NOTIFICATION_EMAIL = team@example.com
```

### Trigger UI Tests

```bash
# In commit message
git commit -m "feat: add new page [run-ui]"

# Or via workflow dispatch (GitHub UI)
Actions → CI/CD → Run workflow → Set run_ui_tests=true
```

---

## 📚 Documentation

### 🎯 Start Here

| Document | Purpose | Status |
|----------|---------|--------|
| **[README.md](README.md)** | **This file - Quick start guide** | ✅ Current |
| **[ARCHITECTURE.md](docs/ARCHITECTURE.md)** | **Framework design, Screenplay Pattern, Questions** | ✅ v2.0 |
| **[FILE-STRUCTURE-GUIDE.md](docs/FILE-STRUCTURE-GUIDE.md)** | **Complete file structure + code templates** | ✅ v2.0 |

### 📖 Configuration & Setup

| Document | Purpose | Status |
|----------|---------|--------|
| **[CONFIGURATION.md](docs/CONFIGURATION.md)** | **Complete configuration guide** (Environment Variables, .env, User Secrets, CI/CD) | ✅ Complete |
| **[RUNBOOK.md](docs/RUNBOOK.md)** | **Troubleshooting & common issues** | ✅ Updated |

### 🧪 Testing & Patterns

| Document | Purpose | Status |
|----------|---------|--------|
| **[STEP-BINDINGS-ARCHITECTURE.md](docs/STEP-BINDINGS-ARCHITECTURE.md)** | **Modular step bindings + Questions examples** | ✅ v2.0 |
| **[ABILITIES-GUIDE.md](docs/ABILITIES-GUIDE.md)** | **Screenplay Abilities pattern guide** | ✅ Complete |
| **[ALLURE-QUICKSTART.md](docs/ALLURE-QUICKSTART.md)** | **Allure reporting guide** (Screenshots, videos, best practices) | ✅ Complete |
| **[SCREENSHOTS-VIDEOS-GUIDE.md](docs/SCREENSHOTS-VIDEOS-GUIDE.md)** | **Evidence capture configuration** | ✅ Complete |

### 🏗️ Architecture Decision Records (ADRs)

| ADR | Decision | Status |
|-----|----------|--------|
| [ADR 001](docs/ADRs/001-playwright-over-selenium.md) | Why Playwright over Selenium | ✅ Approved |
| [ADR 002](docs/ADRs/002-hybrid-screenplay-pom.md) | Hybrid Screenplay + POM approach | ✅ Approved |
| [ADR 003](docs/ADRs/003-allure-reporting.md) | Allure as reporting solution | ✅ Approved |
| [ADR 004](docs/ADRs/004-dependency-injection-autofac.md) | DI with Autofac + auto-registration | ✅ Approved |

### 📝 Contributing

| Document | Purpose | Status |
|----------|---------|--------|
| **[CONTRIBUTING.md](docs/CONTRIBUTING.md)** | **Coding standards, PR process, guidelines** | ✅ Complete |

---

## 🎯 Examples

### Quick Example: UI Testing with Questions Pattern ✨

```gherkin
Scenario: Add items to todo list
  Given the user navigates to the Playwright demo page
  When the user adds "Buy groceries" to the list
  Then the list should contain 1 item
```

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

**More Examples**: API testing, Unit testing, complete scenarios → [FILE-STRUCTURE-GUIDE.md](docs/FILE-STRUCTURE-GUIDE.md) and [STEP-BINDINGS-ARCHITECTURE.md](docs/STEP-BINDINGS-ARCHITECTURE.md)

---

## 🤝 Contributing

We welcome contributions! Please see **[CONTRIBUTING.md](docs/CONTRIBUTING.md)** for:
- ✅ Coding standards & conventions
- ✅ Branch naming strategy
- ✅ Pull request process
- ✅ Adding new features (Pages, Questions, Abilities, Tasks)
- ✅ Testing guidelines
- ✅ Documentation requirements

### Quick Contribution Workflow

```bash
# 1. Fork and clone
git clone <your-fork-url>
cd QuantumTest-Suite

# 2. Create feature branch
git checkout -b feature/amazing-feature

# 3. Make changes and test
dotnet test

# 4. Commit with conventional commits
git commit -m 'feat: add TheAttribute question for CSS attributes'

# 5. Push and create PR
git push origin feature/amazing-feature
# Then open PR on GitHub
```

**Conventional Commits**:
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation only
- `test:` - Adding tests
- `refactor:` - Code refactoring
- `chore:` - Build/tooling changes

---

## 🛠️ Troubleshooting

**⚠️ Complete troubleshooting guide**: [RUNBOOK.md](docs/RUNBOOK.md)

### Quick Fixes

| Issue | Solution | Command |
|-------|----------|---------|
| **Browsers not found** | Install Playwright browsers | `pwsh -c "npx playwright install --with-deps chromium"` |
| **UI tests skipped** | Enable UI tests | `$env:RUN_UI_TESTS="true"` |
| **Secrets error** | Set environment + create config | `$env:DOTNET_ENVIRONMENT="Development"` |
| **Build errors** | Clean and restore | `dotnet clean; dotnet restore; dotnet build` |
| **Port conflicts** | Kill process on port | `Get-Process -Id (Get-NetTCPConnection -LocalPort 5050).OwningProcess \| Stop-Process` |

### Common Issues

**❌ "Executable doesn't exist at ..."**
```powershell
# Install Playwright browsers
pwsh -c "npx playwright install --with-deps chromium"
```

**❌ "UI tests are being skipped"**
```powershell
# Enable UI tests
$env:RUN_UI_TESTS="true"
dotnet test --filter "Category=ui"
```

**❌ "Cannot find appsettings.Development.json"**
```powershell
# Set environment
$env:DOTNET_ENVIRONMENT="Development"

# Or create .env file
copy .env.example .env
```

**❌ "Compilation errors in Questions"**
```powershell
# Make sure you're using .NET 8
dotnet --version  # Should be 8.0.x

# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

---

## 📊 Framework Status

**Build**: ✅ Passing (0 errors, 0 warnings) | **Tests**: ✅ 14/14 (100%) | **Documentation**: ✅ Complete v2.0

📖 **Detailed Metrics**: See [docs/FRAMEWORK_STATUS.md](docs/FRAMEWORK_STATUS.md) for full quality report.

## 📞 Support

- **Documentation**: See [docs/](docs/) directory
- **Issues**: [GitHub Issues](https://github.com/your-org/quantum-test-suite/issues)

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- **Playwright** - Modern browser automation
- **Reqnroll** - BDD framework (SpecFlow successor)
- **Allure** - Beautiful test reporting
- **Autofac** - Powerful dependency injection
- **NUnit** - Robust test framework

---

**Built with ❤️ by QA Engineers, for QA Engineers**
