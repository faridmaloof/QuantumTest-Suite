# 🚀 Quantum Test Suite

Enterprise-grade test automation framework built on **.NET 8**, implementing **BDD with SpecFlow**, **UI/API testing with Playwright**, and **comprehensive reporting with Allure**.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Playwright](https://img.shields.io/badge/Playwright-1.57.0-2EAD33?logo=playwright)](https://playwright.dev/dotnet/)
[![SpecFlow](https://img.shields.io/badge/SpecFlow-3.9.74-3CB371?logo=specflow)](https://specflow.org/)
[![Allure](https://img.shields.io/badge/Allure-2.14.1-FF6C37?logo=allure)](https://docs.qameta.io/allure/)

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
✅ **BDD with Gherkin** - Business-readable test scenarios  
✅ **UI Automation** - Cross-browser testing with Playwright  
✅ **API Testing** - REST API validation with same tool  
✅ **Unit Tests** - Framework components validation with Allure integration  
✅ **Hybrid Architecture** - Screenplay + Page Object Model  
✅ **Dependency Injection** - Testable, maintainable code  
✅ **Rich Reporting** - Allure with screenshots, videos, and trends  
✅ **Multi-Environment** - Dev/Staging/Production configs  
✅ **Docker Support** - Reproducible execution environments  
✅ **GitHub Actions** - Ready-to-use CI/CD pipelines  

### Test Structure
- **UI Tests**: Browser-based scenarios with Playwright
- **API Tests**: RESTful service validation
- **Unit Tests**: Framework utilities, config, factories validation

### Evidence Capture (Configurable)
- **Screenshots**: Before/After steps or on failure
- **Videos**: Full scenario recording
- **API Details**: Request/Response with headers
- **Logs**: Structured console output

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
│  - Modular: Api/, Ui/, UnitFeatures/            │
│  - Base Classes: Auto Evidence Capture          │
└────────┬─────────────────────────┬──────────────┘
         │                          │
         ▼                          ▼
┌────────────────┐         ┌────────────────┐
│  SERVICE LAYER │         │  SCREENPLAY    │
│  (API Logic)   │         │  + POM         │
└────────┬───────┘         └────────┬───────┘
         │                          │
         ▼                          ▼
┌────────────────┐         ┌────────────────┐
│  API CLIENTS   │         │  PAGE OBJECTS  │
└────────┬───────┘         └────────┬───────┘
         │                          │
         └──────────┬───────────────┘
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

### Step Bindings Modular Structure

**NEW: Professional, maintainable organization** 🎯

```
Tests/StepBindings/
├── Base/                                   # 🔹 Automatic Evidence Capture
│   ├── ApiStepBindingsBase.cs             #   → Auto API request/response logging
│   └── UiStepBindingsBase.cs              #   → Auto screenshot/video capture
├── Api/                                    # 🔹 API Test Step Bindings
│   ├── HttpBinStepBindings.cs             #   → HttpBin GET tests
│   ├── RestfulBookerStepBindings.cs       #   → Booking CRUD tests
│   ├── GitHubStepBindings.cs              #   → GitHub search tests
│   └── ApiCommonStepBindings.cs           #   → Common API assertions
├── Ui/                                     # 🔹 UI Test Step Bindings
│   ├── SauceDemoStepBindings.cs           #   → SauceDemo login flow
│   ├── UltimateQaStepBindings.cs          #   → Form submission tests
│   └── GitHubUiStepBindings.cs            #   → E2E API + UI tests
└── UnitFeatures/                           # 🔹 Unit Test Step Bindings
    ├── AbilitiesTestsStepBindings.cs
    ├── ConfigManagerStepBindings.cs
    ├── TestDataFactoryStepBindings.cs
    └── UtilitiesStepBindings.cs
```

**Benefits:**
- ✅ **Automatic Evidence**: No manual screenshot/logging calls
- ✅ **Small Files**: 40-80 lines each (vs 300+ monolithic)
- ✅ **Single Responsibility**: One feature per file
- ✅ **Easy Maintenance**: Clear, focused, professional code
- ✅ **Auto Registration**: Assembly scanning (no DI config changes)

**Key Patterns:**
- **Inheritance Pattern**: Base classes handle evidence capture
- **Factory Pattern**: Dynamic test data generation (Bogus)
- **Service Layer**: Business logic encapsulation
- **Dependency Injection**: Autofac for IoC with auto-registration
- **Type-Safe Contexts**: No magic strings
- **Utilities**: Retry, Wait helpers with centralized timeouts

📖 **Detailed Guide**: [STEP-BINDINGS-ARCHITECTURE.md](docs/STEP-BINDINGS-ARCHITECTURE.md)

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

| Document | Purpose |
|----------|---------|
| **[CONFIGURATION.md](docs/CONFIGURATION.md)** | **📋 Guía completa de configuración** (variables, .env, User Secrets, CI/CD) |
| **[ALLURE-QUICKSTART.md](docs/ALLURE-QUICKSTART.md)** | **📊 Guía de reportes Allure** (generación, evidencias, buenas prácticas) |
| **[STEP-BINDINGS-ARCHITECTURE.md](docs/STEP-BINDINGS-ARCHITECTURE.md)** | **🏗️ Arquitectura modular de step bindings** (base classes, automatic evidence) |
| [ARCHITECTURE.md](docs/ARCHITECTURE.md) | Framework design, patterns, tech stack |
| [CONTRIBUTING.md](docs/CONTRIBUTING.md) | Coding standards, PR process, guidelines |
| [RUNBOOK.md](docs/RUNBOOK.md) | Troubleshooting, common issues, commands |
| [ADR 001](docs/ADRs/001-playwright-over-selenium.md) | Why Playwright over Selenium |
| [ADR 002](docs/ADRs/002-hybrid-screenplay-pom.md) | Hybrid Screenplay + POM approach |
| [ADR 003](docs/ADRs/003-allure-reporting.md) | Allure as reporting solution |
| [ADR 004](docs/ADRs/004-dependency-injection-autofac.md) | DI with Autofac |

---

## 🎯 Examples

### API Testing

```gherkin
@api @smoke
Scenario: Create booking successfully
  Given un payload válido de booking
  When se envía POST /booking
  Then el status debe ser 200
  And el response contiene bookingid
```

### UI Testing

```gherkin
@ui @smoke
Scenario: Login with valid credentials
  Given el usuario está en la página de SauceDemo
  When ingresa credenciales válidas
  Then debe ver la página de inventario
```

### Unit Testing

```gherkin
@unit @smoke
Scenario: ConfigManager loads environment variables correctly
  Given el sistema tiene ConfigManager inicializado
  When se establece variable de entorno "PLAYWRIGHT_HEADLESS=false"
  Then ConfigManager.Settings.Playwright.Headless debe ser "false"
```

### Combined UI + API

```gherkin
@api @ui @e2e
Scenario: Search GitHub user via API then verify in UI
  Given un usuario de GitHub llamado 'octocat' existe via API
  When busca el usuario en UI
  Then el resultado muestra el usuario
```

---

## 🤝 Contributing

We welcome contributions! Please see [CONTRIBUTING.md](docs/CONTRIBUTING.md) for:
- Coding standards
- Branch strategy
- Pull request process
- Adding new features
- Best practices

### Quick Contribution Guide

1. Fork the repository
2. Create feature branch (`feature/amazing-feature`)
3. Commit changes (`git commit -m 'feat: add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

---

## 🛠️ Troubleshooting

**Common issues and solutions are in [RUNBOOK.md](docs/RUNBOOK.md)**

Quick fixes:
- **Browsers not found**: `pwsh -c "npx playwright install --with-deps chromium"`
- **UI tests skipped**: `$env:RUN_UI_TESTS="true"`
- **Secrets error**: Set `DOTNET_ENVIRONMENT=Development` and create `appsettings.Development.json`
- **Allure report empty**: Run tests first, then generate report

---

## 📞 Support

- **Issues**: [Create GitHub Issue](../../issues)
- **Questions**: Use `question` label
- **Security**: Report privately to security team

---

## 📄 License

[Specify your license here]

---

## 👥 Authors

- **SDET Lead** - Framework Architecture
- **QA Team** - Test Scenarios and Maintenance

---

## 🙏 Acknowledgments

- [Playwright Team](https://github.com/microsoft/playwright-dotnet)
- [SpecFlow Community](https://specflow.org/)
- [Allure Framework](https://github.com/allure-framework)
- [Bogus Library](https://github.com/bchavez/Bogus)

---

**Built with ❤️ by the QA Team**
