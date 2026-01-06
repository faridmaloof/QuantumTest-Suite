# Guía de Mejoras en Reportes de Allure

## 📊 Resumen de Cambios

Se han implementado mejoras significativas para organizar mejor los reportes de Allure con información de tendencias, executors, categorías detalladas y metadata estructurada.

---

## ✨ Nuevas Características

### 1. **Información de Environment**
Ahora los reportes incluyen información detallada del entorno de ejecución:

**Ubicación:** `AllureResults/environment.properties`

**Información capturada:**
- Nombre del entorno (Development/QA/Production)
- Versión del framework
- Versión de .NET
- Sistema operativo
- Máquina y usuario
- Configuración de Browser (tipo, headless mode)
- Configuración de screenshots y videos
- Fecha/hora de ejecución

### 2. **Información de Executor (CI/CD)**
Se detecta automáticamente el sistema CI/CD y genera metadata para integración:

**Ubicación:** `AllureResults/executor.json`

**Sistemas soportados:**
- ✅ GitHub Actions
- ✅ Azure DevOps
- ✅ Jenkins
- ✅ GitLab CI
- ✅ Ejecución Local

**Información incluida:**
- Nombre del executor
- URL del build
- Número de build
- Links directos a los resultados

### 3. **Categorías Mejoradas con Emojis**
Se han expandido y mejorado las categorías de errores:

| Categoría | Emoji | Descripción |
|-----------|-------|-------------|
| Product Bugs | 🐛 | Fallos de assertions - defectos de la aplicación |
| API Issues | 🌐 | Errores HTTP, timeouts, respuestas inválidas |
| UI/Browser Issues | 🖥️ | Problemas de locators, timeouts de Playwright |
| Infrastructure Issues | 🔧 | Problemas de red, sistema, ambiente |
| Configuration Errors | ⚙️ | Configuración faltante o inválida |
| Test Data Issues | 📊 | Problemas con factories o datos de prueba |
| Authentication Issues | 🔐 | Login, autenticación, autorización |
| Ignored Tests | ⏭️ | Tests que fueron ignorados |

### 4. **Tags de Allure en Features**
Todos los archivos `.feature` ahora tienen tags de Allure para mejor organización:

**Tags agregados:**
```gherkin
@allure.parentSuite:Unit-Tests
@allure.suite:Configuration
@allure.feature:Config-Manager
@allure.owner:QA-Team
@allure.story:Parsing-Booleans
@allure.severity:critical
```

**Estructura de organización:**
```
Parent Suite (Nivel superior)
  └── Suite (Módulo)
      └── Feature (Funcionalidad)
          └── Story (Historia específica)
              └── Scenario (Escenario de prueba)
```

**Ejemplo de jerarquía:**
```
Unit-Tests
  └── Configuration
      └── Config-Manager
          └── Parsing-Booleans
              └── "Validar parsing de variables booleanas"
```

---

## 🗂️ Organización del Reporte

### Parent Suites (Nivel 1)
- **Unit-Tests**: Tests unitarios del framework
- **API-Tests**: Tests de APIs REST
- **UI-Tests**: Tests de interfaz de usuario

### Suites (Nivel 2)
**Unit Tests:**
- Configuration
- Utilities
- Test-Data
- Screenplay-Pattern

**API Tests:**
- HTTPBin
- GitHub-API
- Restful-Booker

**UI Tests:**
- Forms
- Navigation
- Interactions

### Features (Nivel 3)
Define la funcionalidad específica siendo probada:
- Config-Manager
- Helper-Functions
- Data-Factories
- User-Search
- CRUD-Operations
- Contact-Form

### Stories (Nivel 4)
Define la historia de usuario o caso de uso específico:
- RetryHelper
- BookingFactory
- Search-User-by-Username
- Create-Booking
- Submit-Contact-Form

### Severities
- **critical**: Funcionalidad core, debe pasar siempre
- **normal**: Funcionalidad importante pero no crítica
- **minor**: Mejoras o casos edge
- **trivial**: Tests exploratorios

---

## 🔧 Archivos Creados

### 1. AllureEnvironmentWriter.cs
```
Tests/Framework/Core/Reporting/AllureEnvironmentWriter.cs
```

**Propósito:** Genera `environment.properties` con información del entorno.

**Cuándo se ejecuta:** Al inicio de cada ejecución de tests (BeforeTestRun).

**Ejemplo de salida:**
```properties
Test.Environment=Development
Framework=QuantumTestSuite
Framework.Version=1.0.0
DotNet.Version=8.0.11
OS=Microsoft Windows NT 10.0.22631.0
Machine=DEV-PC
User=developer
Browser=chromium
Headless=true
Video.Enabled=false
Screenshot.OnFailure=true
Execution.Date=2025-12-30 14:30:00
```

### 2. AllureExecutorWriter.cs
```
Tests/Framework/Core/Reporting/AllureExecutorWriter.cs
```

**Propósito:** Genera `executor.json` con información del CI/CD.

**Cuándo se ejecuta:** Al inicio de cada ejecución de tests (BeforeTestRun).

**Ejemplo de salida (GitHub Actions):**
```json
{
  "name": "GitHub Actions",
  "type": "github",
  "url": "https://github.com/user/repo/actions/runs/12345",
  "buildOrder": "42",
  "buildName": "#42",
  "buildUrl": "https://github.com/user/repo/actions/runs/12345",
  "reportName": "Run #42"
}
```

**Ejemplo de salida (Local):**
```json
{
  "name": "Local",
  "type": "local",
  "buildName": "Local-20251230-143000",
  "reportName": "Local Execution - 2025-12-30 14:30:00"
}
```

### 3. AllureCategoriesWriter.cs
```
Tests/Framework/Core/Reporting/AllureCategoriesWriter.cs
```

**Propósito:** Genera `categories.json` con categorías de errores mejoradas.

**Cuándo se ejecuta:** Al inicio de cada ejecución de tests (BeforeTestRun).

**Ejemplo de salida:**
```json
[
  {
    "name": "🐛 Product Bugs",
    "description": "Test failures due to application defects",
    "matchedStatuses": ["failed"],
    "messageRegex": ".*(AssertionException|AssertionError|Expected.*but was).*"
  },
  {
    "name": "🌐 API Issues",
    "description": "API-related failures",
    "matchedStatuses": ["broken", "failed"],
    "messageRegex": ".*(HttpRequestException|WebException|API|HTTP).*"
  }
]
```

---

## 📝 Cambios en Archivos Existentes

### TestHooks.cs
**Cambio:** Se agregó la generación automática de archivos de metadata.

```csharp
[BeforeTestRun(Order = -100)]
public static void EnsureAllureDirectories()
{
    // ... código existente ...
    
    // Write Allure metadata files for better reporting
    AllureEnvironmentWriter.WriteEnvironmentInfo(allureDir);
    AllureExecutorWriter.WriteExecutorInfo(allureDir);
    AllureCategoriesWriter.WriteCategoriesFile(allureDir);
}
```

### allureConfig.json
**Cambios:**
- Agregado `title` para el reporte
- Categorías expandidas de 4 a 8
- Agregados emojis a nombres de categorías
- Agregadas descripciones detalladas

### Archivos .feature
**Cambios en TODOS los archivos .feature:**
- Agregados tags `@allure.parentSuite`
- Agregados tags `@allure.suite`
- Agregados tags `@allure.feature`
- Agregados tags `@allure.owner`
- Agregados tags `@allure.story` a cada escenario
- Agregados tags `@allure.severity` a cada escenario

**Archivos modificados:**
- `ConfigManagerTests.feature`
- `UtilitiesTests.feature`
- `TestDataFactoryTests.feature`
- `AbilitiesTests.feature`
- `httpbin_get.feature`
- `github_user_search.feature`
- `restful_booker_crud.feature`
- `ultimateqa_form.feature`

---

## 🎯 Cómo Ver las Mejoras

### 1. Ejecutar Tests
```powershell
dotnet test --filter "Category=unit"
```

### 2. Generar Reporte de Allure
```powershell
./scripts/allure-report.ps1 -Open
```

### 3. Explorar el Reporte

**Behaviors Tab:**
- Ahora verás jerarquía completa: Parent Suite → Suite → Feature → Story
- Tests organizados por funcionalidad
- Fácil navegación por módulos

**Categories Tab:**
- Verás las 8 categorías con emojis
- Errores clasificados automáticamente
- Descripción clara del tipo de fallo

**Suites Tab:**
- Organización por Parent Suite (Unit/API/UI)
- Suites agrupadas lógicamente
- Metadata de owner visible

**Environment Widget:**
- Información completa del entorno
- Configuración de ejecución
- Versiones de framework y runtime

**Executors Widget (si aplica):**
- Link directo al build en CI/CD
- Número de build
- Sistema de CI/CD utilizado

**Timeline Tab:**
- Duración de cada test
- Orden de ejecución
- Tests en paralelo visibles

---

## 🎨 Antes vs Después

### Antes
```
❌ Sin información de environment
❌ Sin información de executor
❌ 4 categorías básicas sin descripción
❌ Tests sin jerarquía clara
❌ Sin metadata de owner/severity
❌ Organización plana
```

### Después
```
✅ Environment completo con 12+ propiedades
✅ Executor con links a CI/CD
✅ 8 categorías detalladas con emojis y descripciones
✅ Jerarquía de 4 niveles: Parent Suite → Suite → Feature → Story
✅ Metadata completa: owner, severity, story
✅ Organización jerárquica clara
```

---

## 📊 Ejemplo de Visualización

### En el Reporte de Allure verás:

**Dashboard:**
```
╔══════════════════════════════════════════╗
║ QuantumTestSuite - Test Automation      ║
║ Report                                   ║
║                                          ║
║ Total: 17  Passed: 16  Failed: 1        ║
║ Duration: 17.0s                          ║
║                                          ║
║ Environment: Development                 ║
║ Browser: chromium (headless)             ║
║ Executor: Local Execution                ║
╚══════════════════════════════════════════╝
```

**Behaviors:**
```
📦 Unit-Tests
  ├─ Configuration
  │   └─ Config-Manager
  │       ├─ ✅ Configuración carga valores por defecto
  │       ├─ ✅ ConfigManager es singleton
  │       └─ ✅ Validar parsing de variables booleanas (6 tests)
  ├─ Utilities
  │   └─ Helper-Functions
  │       ├─ ✅ RetryHelper ejecuta acción exitosa
  │       ├─ ✅ RetryHelper reintenta hasta maxAttempts
  │       └─ ✅ WaitHelper espera por condición
  └─ Test-Data
      └─ Data-Factories
          ├─ ✅ BookingFactory genera datos válidos
          └─ ❌ BookingFactory genera datos únicos

📦 API-Tests
  ├─ HTTPBin
  │   └─ GET-Requests
  │       └─ ✅ GET /get returns 200
  ├─ GitHub-API
  │   └─ User-Search
  │       └─ ✅ Buscar octocat via API
  └─ Restful-Booker
      └─ CRUD-Operations
          └─ ✅ Create booking and validate

📦 UI-Tests
  └─ Forms
      └─ Contact-Form
          └─ Submit-Contact-Form (ignored)
```

**Categories:**
```
🐛 Product Bugs: 0 tests
   Test failures due to application defects

🌐 API Issues: 0 tests
   API-related failures (HTTP errors, timeouts)

🖥️ UI/Browser Issues: 0 tests
   Browser automation failures

🔧 Infrastructure Issues: 0 tests
   Environment or network-related failures

⚙️ Configuration Errors: 0 tests
   Missing or invalid configuration

📊 Test Data Issues: 1 test
   Problems with test data or factories
   → BookingFactory genera datos únicos

🔐 Authentication Issues: 0 tests
   Login or authorization failures

⏭️ Ignored Tests: 12 tests
   Tests that were skipped
```

---

## 🚀 Próximos Pasos Recomendados

### 1. Generar Trends History ✅ (Ya Configurado)

El script `allure-report.ps1` **automáticamente** preserva el historial para tendencias.

**Cómo funciona:**
- Al ejecutar `./scripts/allure-report.ps1`, el script copia el directorio `history` del reporte anterior antes de generar uno nuevo
- Esto permite que Allure mantenga las tendencias entre ejecuciones
- Después de 2-3 ejecuciones, verás gráficos de tendencias en el reporte

**Para ver tendencias:**
```powershell
# Ejecutar tests y generar reporte (repetir 2-3 veces)
dotnet test --filter "Category=unit"
./scripts/allure-report.ps1 -Open

# Cada ejecución agregará un punto de datos al historial
```

**Widgets de Tendencias que verás:**
- **Duration Trend**: Duración de ejecución a lo largo del tiempo
- **History Trend**: Cantidad de tests passed/failed/broken en cada ejecución
- **Retry Trend**: Tests que fallaron y fueron reintentados
- **Categories Trend**: Distribución de categorías de errores en el tiempo

**Limpiar historial (opcional):**
```powershell
# Empezar desde cero
Remove-Item "Reports/AllureReport/history" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "Tests/bin/Debug/net8.0/Reports/AllureResults/history" -Recurse -Force -ErrorAction SilentlyContinue
```

### 2. Integrar con CI/CD
Ya tienes los executors configurados. Solo necesitas:
- GitHub Actions: Ya configurado en `.github/workflows/ci-cd.yml`
- Azure DevOps: Agregar task de Allure
- Jenkins: Agregar plugin de Allure

### 3. Personalizar Categories
Edita `AllureCategoriesWriter.cs` para agregar categorías específicas de tu proyecto:

```csharp
new AllureCategory
{
    Name = "💳 Payment Issues",
    Description = "Payment gateway failures",
    MatchedStatuses = new[] { "failed", "broken" },
    MessageRegex = ".*(Payment|Transaction|Card|Stripe|PayPal).*"
}
```

### 4. Agregar Links a Issues
Cuando un test falla, puedes agregar links a Jira/GitHub Issues:

```csharp
[AllureLink("JIRA-123")]
[AllureIssue("BUG-456")]
[Then(@"el booking debe tener firstname no vacío")]
public void ThenBookingTieneFirstname()
{
    // ...
}
```

---

## 🤝 Owners Configurados

Los owners están configurados por equipo:

| Owner | Suite | Responsabilidad |
|-------|-------|-----------------|
| QA-Team | Configuration | Tests de configuración |
| Framework-Team | Utilities, Test-Data, Screenplay | Framework y herramientas |
| API-Team | HTTPBin, GitHub-API, Restful-Booker | APIs públicas |
| UI-Team | Forms, Navigation | Interfaces de usuario |

---

## 📚 Referencias

- [Allure Behaviors](https://docs.qameta.io/allure/#_behaviors)
- [Allure Categories](https://docs.qameta.io/allure/#_categories)
- [Allure Environment](https://docs.qameta.io/allure/#_environment)
- [Allure Executors](https://docs.qameta.io/allure/#_executor)

---

**¿Preguntas?** Consulta [ALLURE-QUICKSTART.md](ALLURE-QUICKSTART.md) para guías de uso básico.
