# Configuration Guide

Esta guía explica **todas las opciones** disponibles para configurar el framework de automatización. Puedes usar cualquiera de ellas según tu contexto de uso.

---

## 📋 Tabla de Contenidos

- [Opciones Disponibles](#opciones-disponibles)
- [Prioridad de Configuración](#prioridad-de-configuración)
- [Variables Requeridas y Opcionales](#variables-requeridas-y-opcionales)
- [Opción 1: Variables de Entorno Manuales](#opción-1-variables-de-entorno-manuales)
- [Opción 2: Archivo .env](#opción-2-archivo-env)
- [Opción 3: User Secrets (.NET)](#opción-3-user-secrets-net)
- [Opción 4: CI/CD Secrets](#opción-4-cicd-secrets)
- [Configuraciones Específicas por Entorno](#configuraciones-específicas-por-entorno)
- [Validar Configuración](#validar-configuración)

---

## Opciones Disponibles

El framework soporta **múltiples formas de configuración** que pueden coexistir:

| Opción | Uso Recomendado | Prioridad |
|--------|-----------------|-----------|
| **Variables de Entorno** | CI/CD, Overrides temporales | ⭐⭐⭐⭐⭐ (Máxima) |
| **User Secrets** | Desarrollo local con credenciales | ⭐⭐⭐⭐ |
| **.env.{environment}** | Configuración por entorno (dev/qa/prod) | ⭐⭐⭐ |
| **.env** | Configuración base local | ⭐⭐ |
| **appsettings.json** | Valores por defecto (estructura) | ⭐ (Mínima) |

**✅ Puedes usar todas simultáneamente**. El framework las combinará según la prioridad indicada.

---

## Prioridad de Configuración

El orden de precedencia (de mayor a menor):

```
Environment Variables (CI/CD, export manual)
         ▼
    User Secrets (.NET)
         ▼
 .env.{TEST_ENVIRONMENT}
         ▼
       .env
         ▼
  appsettings.json
```

**Ejemplo:**
- Si defines `PLAYWRIGHT_HEADLESS=false` en variables de entorno
- Y tienes `PLAYWRIGHT_HEADLESS=true` en `.env`
- **Se usará `false`** (variables de entorno tienen mayor prioridad)

---

## Variables Requeridas y Opcionales

### ✅ Variables Opcionales (tienen defaults)

Todas las variables son **opcionales** porque tienen valores por defecto:

```bash
# Environment
TEST_ENVIRONMENT=local          # Default: "local"

# Playwright (Browser)
PLAYWRIGHT_HEADLESS=true        # Default: true
PLAYWRIGHT_BROWSER=chromium     # Default: "chromium"
PLAYWRIGHT_BASE_URL=https://www.saucedemo.com/
PLAYWRIGHT_SLOWMO=0             # Default: 0 (sin delay)
PLAYWRIGHT_VIDEO_ENABLED=false  # Default: false

# Screenshots
SCREENSHOT_BEFORE_STEP=false    # Default: false
SCREENSHOT_AFTER_STEP=false     # Default: false
SCREENSHOT_ON_FAILURE=true      # Default: true

# API Endpoints (tienen defaults públicos)
API_HTTPBIN=https://httpbin.org
API_RESTFUL_BOOKER=https://restful-booker.herokuapp.com
API_GITHUB=https://api.github.com
API_GH_USERS_SEARCH_UI=https://gh-users-search.netlify.app/

# Test Control
RUN_UI_TESTS=false              # Default: false (solo API tests)

# Allure Reporting
ALLURE_RESULTS_DIRECTORY=Reports/AllureResults
ALLURE_CLEANUP_CYCLE=1
```

### ⚠️ Variables Sensibles (credenciales)

Estas deben configurarse para tests que requieran autenticación:

```bash
# SauceDemo Credentials (para tests UI de login)
SAUCEDEMO_USERNAME=standard_user
SAUCEDEMO_PASSWORD=secret_sauce
```

**⚠️ NUNCA commits credenciales reales en el repositorio**

---

## Opción 1: Variables de Entorno Manuales

### Windows (PowerShell)

```powershell
# Establecer para la sesión actual
$env:TEST_ENVIRONMENT="qa"
$env:PLAYWRIGHT_HEADLESS="false"
$env:SAUCEDEMO_USERNAME="standard_user"
$env:SAUCEDEMO_PASSWORD="secret_sauce"
$env:RUN_UI_TESTS="true"

# Ejecutar tests
dotnet test
```

### Windows (CMD)

```cmd
set TEST_ENVIRONMENT=qa
set PLAYWRIGHT_HEADLESS=false
set SAUCEDEMO_USERNAME=standard_user
set SAUCEDEMO_PASSWORD=secret_sauce
dotnet test
```

### Linux / macOS

```bash
export TEST_ENVIRONMENT="qa"
export PLAYWRIGHT_HEADLESS="false"
export SAUCEDEMO_USERNAME="standard_user"
export SAUCEDEMO_PASSWORD="secret_sauce"
dotnet test
```

### Variables de Sistema (Permanentes)

**Windows:**
```powershell
# Establecer a nivel de usuario (persistente)
[Environment]::SetEnvironmentVariable("TEST_ENVIRONMENT", "local", "User")
[Environment]::SetEnvironmentVariable("PLAYWRIGHT_BROWSER", "chromium", "User")
```

**Linux/macOS:**
```bash
# Agregar a ~/.bashrc o ~/.zshrc
echo 'export TEST_ENVIRONMENT="local"' >> ~/.bashrc
source ~/.bashrc
```

---

## Opción 2: Archivo .env

### Configuración Base

```powershell
# 1. Copiar template
copy .env.example .env

# 2. Editar con tus valores
notepad .env
```

**Contenido de `.env`:**
```bash
# Environment
TEST_ENVIRONMENT=local

# Playwright
PLAYWRIGHT_HEADLESS=true
PLAYWRIGHT_BROWSER=chromium
PLAYWRIGHT_BASE_URL=https://www.saucedemo.com/
PLAYWRIGHT_VIDEO_ENABLED=false

# Screenshots
SCREENSHOT_BEFORE_STEP=false
SCREENSHOT_AFTER_STEP=false
SCREENSHOT_ON_FAILURE=true

# Credentials (⚠️ sensible)
SAUCEDEMO_USERNAME=standard_user
SAUCEDEMO_PASSWORD=secret_sauce

# Test Control
RUN_UI_TESTS=false

# APIs
API_HTTPBIN=https://httpbin.org
API_RESTFUL_BOOKER=https://restful-booker.herokuapp.com
API_GITHUB=https://api.github.com
```

### Configuraciones por Entorno

```powershell
# Crear archivo para QA
copy .env.qa.example .env.qa

# Editar configuración QA
notepad .env.qa

# Ejecutar con configuración QA
$env:TEST_ENVIRONMENT="qa"
dotnet test
```

**El framework cargará automáticamente `.env.qa` cuando `TEST_ENVIRONMENT=qa`**

### ⚠️ Seguridad con .env

```bash
# ✅ NUNCA hacer commit de archivos .env reales
# .gitignore ya excluye:
.env
.env.local
.env.development
.env.qa
.env.staging
.env.production

# ✅ Solo hacer commit de templates (.example)
!.env.example
!.env.*.example
```

---

## Opción 3: User Secrets (.NET)

**Ideal para desarrollo local** sin exponer credenciales en archivos.

### Ventajas
✅ **No se incluyen en el repositorio** (almacenados fuera del proyecto)  
✅ **Específicos por máquina/usuario**  
✅ **Sincronización con .NET tooling** (Visual Studio, Rider)

### Configurar User Secrets

#### Paso 1: Inicializar User Secrets (ya configurado)

```powershell
cd Tests
dotnet user-secrets init
```

El proyecto ya tiene configurado el `UserSecretsId` en `QuantumTestSuite.Tests.csproj`:
```xml
<UserSecretsId>quantum-testsuite-3f8a4b2c-9d7e-4a1f-b6c3-2e5d8f1a9b3c</UserSecretsId>
```

#### Paso 2: Agregar Secrets

```powershell
# Navegar a la carpeta Tests
cd Tests

# Agregar credenciales
dotnet user-secrets set "Users:SauceDemo:Username" "standard_user"
dotnet user-secrets set "Users:SauceDemo:Password" "secret_sauce"

# Agregar URLs personalizadas
dotnet user-secrets set "Playwright:BaseUrl" "https://custom-env.com"
dotnet user-secrets set "Apis:RestfulBooker" "https://my-api.com"

# Agregar configuraciones de Playwright
dotnet user-secrets set "Playwright:Headless" "false"
dotnet user-secrets set "Playwright:Browser" "firefox"
dotnet user-secrets set "Playwright:VideoEnabled" "true"

# Screenshots
dotnet user-secrets set "Playwright:ScreenshotOptions:BeforeStep" "true"
dotnet user-secrets set "Playwright:ScreenshotOptions:AfterStep" "true"
```

#### Paso 3: Listar Secrets Configurados

```powershell
cd Tests
dotnet user-secrets list
```

**Salida esperada:**
```
Users:SauceDemo:Username = standard_user
Users:SauceDemo:Password = *******
Playwright:Headless = false
Playwright:Browser = firefox
```

#### Paso 4: Ejecutar Tests (automático)

```powershell
# Los User Secrets se cargan automáticamente
dotnet test
```

### Borrar User Secrets

```powershell
# Borrar un secret específico
dotnet user-secrets remove "Users:SauceDemo:Password"

# Borrar todos los secrets
dotnet user-secrets clear
```

### Ubicación de User Secrets

**Windows:**  
```
%APPDATA%\Microsoft\UserSecrets\quantum-testsuite-3f8a4b2c-9d7e-4a1f-b6c3-2e5d8f1a9b3c\secrets.json
```

**Linux / macOS:**  
```
~/.microsoft/usersecrets/quantum-testsuite-3f8a4b2c-9d7e-4a1f-b6c3-2e5d8f1a9b3c/secrets.json
```

### Ejemplo: secrets.json

```json
{
  "Users": {
    "SauceDemo": {
      "Username": "standard_user",
      "Password": "secret_sauce"
    }
  },
  "Playwright": {
    "Headless": false,
    "Browser": "firefox",
    "VideoEnabled": true,
    "ScreenshotOptions": {
      "BeforeStep": true,
      "AfterStep": false
    }
  }
}
```

---

## Opción 4: CI/CD Secrets

### GitHub Actions

**Configurar Secrets:**
1. Ve a: `Settings` → `Secrets and variables` → `Actions`
2. Click `New repository secret`
3. Agrega:

```
SAUCEDEMO_USERNAME = standard_user
SAUCEDEMO_PASSWORD = secret_sauce
```

**Usar en Workflow (.github/workflows/ci-cd.yml):**

```yaml
name: CI/CD

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    env:
      TEST_ENVIRONMENT: qa
      PLAYWRIGHT_HEADLESS: true
      PLAYWRIGHT_BROWSER: chromium
      RUN_UI_TESTS: true
      
      # Secrets desde GitHub
      SAUCEDEMO_USERNAME: ${{ secrets.SAUCEDEMO_USERNAME }}
      SAUCEDEMO_PASSWORD: ${{ secrets.SAUCEDEMO_PASSWORD }}
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Run Tests
        run: dotnet test
```

### Azure DevOps

**Configurar Variables:**
1. Ve a: `Pipelines` → `Library` → `Variable groups`
2. Crea grupo: `QuantumTestSuite-Secrets`
3. Agrega variables (marca como secretas):

```
SAUCEDEMO_USERNAME = standard_user
SAUCEDEMO_PASSWORD = (secret)
```

**Usar en Pipeline (azure-pipelines.yml):**

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

variables:
  - group: QuantumTestSuite-Secrets
  - name: TEST_ENVIRONMENT
    value: 'qa'
  - name: PLAYWRIGHT_HEADLESS
    value: 'true'
  - name: RUN_UI_TESTS
    value: 'true'

steps:
  - task: UseDotNet@2
    inputs:
      version: '8.0.x'

  - script: dotnet restore
    displayName: 'Restore dependencies'

  - script: dotnet test
    displayName: 'Run tests'
    env:
      SAUCEDEMO_USERNAME: $(SAUCEDEMO_USERNAME)
      SAUCEDEMO_PASSWORD: $(SAUCEDEMO_PASSWORD)
```

### Jenkins

**Configurar Credentials:**
1. Ve a: `Manage Jenkins` → `Credentials`
2. Add Credentials (Secret text):
   - ID: `saucedemo-username`
   - Secret: `standard_user`
   - ID: `saucedemo-password`
   - Secret: `secret_sauce`

**Usar en Jenkinsfile:**

```groovy
pipeline {
    agent any
    
    environment {
        TEST_ENVIRONMENT = 'qa'
        PLAYWRIGHT_HEADLESS = 'true'
        RUN_UI_TESTS = 'true'
        PLAYWRIGHT_BROWSER = 'chromium'
        
        // Cargar secrets de Jenkins
        SAUCEDEMO_USERNAME = credentials('saucedemo-username')
        SAUCEDEMO_PASSWORD = credentials('saucedemo-password')
    }
    
    stages {
        stage('Setup') {
            steps {
                sh 'dotnet restore'
            }
        }
        
        stage('Test') {
            steps {
                sh 'dotnet test'
            }
        }
        
        stage('Allure Report') {
            steps {
                allure([
                    results: [[path: 'Tests/bin/Debug/net8.0/Reports/AllureResults']]
                ])
            }
        }
    }
}
```

### GitLab CI/CD

**Configurar Variables:**
1. Ve a: `Settings` → `CI/CD` → `Variables`
2. Agrega (protected + masked):

```
SAUCEDEMO_USERNAME = standard_user
SAUCEDEMO_PASSWORD = (secret)
```

**Usar en .gitlab-ci.yml:**

```yaml
variables:
  TEST_ENVIRONMENT: "qa"
  PLAYWRIGHT_HEADLESS: "true"
  RUN_UI_TESTS: "true"

test:
  stage: test
  image: mcr.microsoft.com/dotnet/sdk:8.0
  
  script:
    - dotnet restore
    - dotnet test
  
  artifacts:
    when: always
    paths:
      - Tests/bin/Debug/net8.0/Reports/AllureResults
```

---

## Configuraciones Específicas por Entorno

### Escenarios Comunes

#### 1. Desarrollo Local (Visible, con Video)

```powershell
# Opción A: .env.development
copy .env.development.example .env.development
# Editar: PLAYWRIGHT_HEADLESS=false, VIDEO_ENABLED=true

$env:TEST_ENVIRONMENT="development"
dotnet test

# Opción B: Variables manuales
$env:PLAYWRIGHT_HEADLESS="false"
$env:PLAYWRIGHT_VIDEO_ENABLED="true"
$env:RUN_UI_TESTS="true"
dotnet test
```

#### 2. QA/Testing (Headless, Screenshots on Failure)

```bash
# .env.qa
TEST_ENVIRONMENT=qa
PLAYWRIGHT_HEADLESS=true
PLAYWRIGHT_BROWSER=chromium
SCREENSHOT_ON_FAILURE=true
SCREENSHOT_BEFORE_STEP=false
RUN_UI_TESTS=true
```

#### 3. CI/CD (Multi-Browser, Fast)

```yaml
# GitHub Actions Matrix
strategy:
  matrix:
    browser: [chromium, firefox, webkit]
    
env:
  PLAYWRIGHT_BROWSER: ${{ matrix.browser }}
  PLAYWRIGHT_HEADLESS: true
  PLAYWRIGHT_VIDEO_ENABLED: false
  SCREENSHOT_ON_FAILURE: true
```

#### 4. Producción (Smoke Tests, Evidence)

```bash
# .env.production
TEST_ENVIRONMENT=production
PLAYWRIGHT_HEADLESS=true
PLAYWRIGHT_BASE_URL=https://prod.example.com
PLAYWRIGHT_VIDEO_ENABLED=true
SCREENSHOT_BEFORE_STEP=true
SCREENSHOT_AFTER_STEP=true
RUN_UI_TESTS=false  # Solo smoke API tests
```

---

## Validar Configuración

### Opción 1: Imprimir Configuración Actual

Agrega en cualquier test:

```csharp
using QuantumTestSuite.Core.Config;

[Test]
public void PrintCurrentConfig()
{
    var settings = ConfigManager.Settings;
    Console.WriteLine($"Environment: {settings.Env}");
    Console.WriteLine($"Headless: {settings.Playwright.Headless}");
    Console.WriteLine($"Browser: {settings.Playwright.Browser}");
    Console.WriteLine($"Video Enabled: {settings.Playwright.VideoEnabled}");
    Console.WriteLine($"BaseUrl: {settings.Playwright.BaseUrl}");
}
```

### Opción 2: Verificar Variables de Entorno

```powershell
# Listar todas las variables configuradas
Get-ChildItem Env: | Where-Object { $_.Name -like "*PLAYWRIGHT*" -or $_.Name -like "*SAUCEDEMO*" }
```

### Opción 3: Logs del Framework

Al ejecutar tests, verás en consola:

```
[ConfigManager] Loaded: .env
[ConfigManager] Loaded: .env.qa
```

---

## Resumen de Buenas Prácticas

✅ **Para desarrollo local**: User Secrets o `.env` (no commitear)  
✅ **Para CI/CD**: Variables de pipeline (secrets cifrados)  
✅ **Para equipos**: `.env.{environment}.example` como template  
✅ **Overrides temporales**: Variables de entorno manuales  
✅ **Nunca commits**: `.env`, archivos con credenciales reales  

---

## Troubleshooting

### "User Secrets not found"

```powershell
cd Tests
dotnet user-secrets init
dotnet user-secrets list
```

### ".env file not loaded"

Verifica que esté en la raíz del proyecto (donde está `QuantumTestSuite.sln`), no dentro de `Tests/`.

### "Environment variable not overriding"

Recuerda el orden de prioridad:
```
Env Vars > User Secrets > .env.{ENV} > .env > appsettings.json
```

Si estableces en `.env` pero no funciona, verifica que no esté configurado en User Secrets o variables de sistema.

---

**¿Tienes dudas?** Consulta [RUNBOOK.md](RUNBOOK.md) para problemas específicos.
