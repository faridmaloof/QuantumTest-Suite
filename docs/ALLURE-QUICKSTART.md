# Allure Reporting - Quickstart Guide

Guía completa para generar, visualizar y gestionar reportes de Allure en el framework de automatización.

---

## 📋 Tabla de Contenidos

- [¿Qué es Allure?](#qué-es-allure)
- [Prerequisitos](#prerequisitos)
- [Generación de Reportes - Local](#generación-de-reportes---local)
- [Generación de Reportes - CI/CD](#generación-de-reportes---cicd)
- [Configuración de Evidencias](#configuración-de-evidencias)
- [Estructura de Resultados](#estructura-de-resultados)
- [Buenas Prácticas](#buenas-prácticas)
- [Troubleshooting](#troubleshooting)

---

## ¿Qué es Allure?

**Allure** es un framework de reportes que genera reportes HTML interactivos y visuales para tests automatizados.

### Características Principales

✅ **Timeline**: Visualización temporal de ejecución  
✅ **Trends**: Histórico de pass/fail rates  
✅ **Categories**: Clasificación de errores (bugs vs infraestructura)  
✅ **Attachments**: Screenshots, videos, JSON, logs  
✅ **Behaviors**: Organización por stories BDD  
✅ **Suites**: Agrupación por features  

### Evidencias Capturadas Automáticamente

**API Tests:**
- Request details (método, URL, headers, body)
- Response details (status, headers, body)

**UI Tests:**
- Screenshots (before/after steps o on failure)
- Videos completos del escenario (opcional)
- Console logs

**Unit Tests:**
- Stack traces
- Assertion details
- Custom attachments

---

## Prerequisitos

### Instalar Allure CLI

**Windows (winget):**
```powershell
winget install QA.Allure
```

**Windows (Scoop):**
```powershell
scoop install allure
```

**macOS (Homebrew):**
```bash
brew install allure
```

**Linux (manual):**
```bash
wget https://github.com/allure-framework/allure2/releases/download/2.24.0/allure-2.24.0.tgz
tar -zxvf allure-2.24.0.tgz
sudo mv allure-2.24.0 /opt/allure
echo 'export PATH=$PATH:/opt/allure/bin' >> ~/.bashrc
source ~/.bashrc
```

### Verificar Instalación

```powershell
allure --version
# Output: 2.24.0 (o superior)
```

---

## Generación de Reportes - Local

### Método 1: Script Automatizado (Recomendado)

```powershell
# Generar y abrir reporte
./scripts/allure-report.ps1 -Open

# Solo generar (sin abrir navegador)
./scripts/allure-report.ps1
```

**Ventajas:**
- ✅ Limpia reportes anteriores
- ✅ **Preserva historial para tendencias automáticamente**
- ✅ Genera nuevo reporte
- ✅ Abre automáticamente en navegador

**Nota sobre Tendencias:**
El script automáticamente copia el directorio `history` del reporte anterior antes de generar uno nuevo. Esto permite que Allure mantenga las tendencias históricas. Después de ejecutar el script 2-3 veces, verás gráficos de tendencias en el reporte.

### Método 2: Comandos Manuales

```powershell
# 1. Ejecutar tests (genera resultados crudos)
dotnet test

# 2. Generar reporte HTML
allure generate Tests/bin/Debug/net8.0/Reports/AllureResults -o Reports/AllureReport --clean

# 3. Abrir reporte en navegador
allure open Reports/AllureReport
```

**Ubicación del reporte:**  
`Reports/AllureReport/index.html`

### Método 3: Docker (Sin instalar Allure)

```powershell
# Iniciar contenedor con reporte
docker-compose up allure-report

# Abrir navegador en: http://localhost:5050
```

---

## Generación de Reportes - CI/CD

### GitHub Actions

**Ya configurado en `.github/workflows/ci-cd.yml`:**

```yaml
- name: Upload Allure Results
  if: always()
  uses: actions/upload-artifact@v4
  with:
    name: allure-results
    path: Tests/bin/Debug/net8.0/Reports/AllureResults
    retention-days: 30

- name: Publish Allure Report
  if: always()
  uses: simple-elly/allure-report-action@v1
  with:
    allure_results: Tests/bin/Debug/net8.0/Reports/AllureResults
    allure_report: allure-report
    allure_history: allure-history
```

**Acceder al reporte:**
1. Ve a: `Actions` → `Workflow run` → `Artifacts`
2. Descarga `allure-report.zip`
3. Extrae y abre `index.html`

**O usa GitHub Pages:**

```yaml
- name: Deploy to GitHub Pages
  uses: peaceiris/actions-gh-pages@v3
  with:
    github_token: ${{ secrets.GITHUB_TOKEN }}
    publish_dir: ./allure-report
```

Reporte disponible en: `https://<user>.github.io/<repo>/`

### Azure DevOps

```yaml
- task: PublishAllureReport@1
  inputs:
    allureVersion: '2.24.0'
    resultsDir: 'Tests/bin/Debug/net8.0/Reports/AllureResults'
    reportDir: 'allure-report'
  condition: always()

- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: 'allure-report'
    ArtifactName: 'allure-report'
  condition: always()
```

### Jenkins

```groovy
post {
    always {
        allure([
            includeProperties: false,
            jdk: '',
            results: [[path: 'Tests/bin/Debug/net8.0/Reports/AllureResults']],
            reportBuildPolicy: 'ALWAYS'
        ])
    }
}
```

**Acceder:** `http://<jenkins-url>/job/<job-name>/<build-number>/allure/`

### GitLab CI/CD

```yaml
test:
  script:
    - dotnet test
  artifacts:
    when: always
    paths:
      - Tests/bin/Debug/net8.0/Reports/AllureResults
    reports:
      junit: Tests/bin/Debug/net8.0/Reports/AllureResults/*.xml

allure:
  stage: report
  image: frankescobar/allure-docker
  script:
    - allure generate Tests/bin/Debug/net8.0/Reports/AllureResults -o allure-report --clean
  artifacts:
    paths:
      - allure-report
  only:
    - main
```

---

## Configuración de Evidencias

### Control de Screenshots

Configurar en `.env`:

```bash
# Capturar antes de cada step (mucho overhead)
SCREENSHOT_BEFORE_STEP=false

# Capturar después de cada step (debugging detallado)
SCREENSHOT_AFTER_STEP=false

# Capturar solo en errores (recomendado para CI/CD)
SCREENSHOT_ON_FAILURE=true
```

**Recomendaciones:**

| Entorno | Before | After | On Failure |
|---------|--------|-------|------------|
| **Local Development** | ❌ | ✅ | ✅ |
| **CI/CD** | ❌ | ❌ | ✅ |
| **Debugging** | ✅ | ✅ | ✅ |

### Control de Videos

```bash
# .env
PLAYWRIGHT_VIDEO_ENABLED=true  # Graba video completo del escenario
```

**⚠️ Advertencias:**
- Videos aumentan significativamente el tiempo de ejecución
- Recomendado solo para debugging o tests críticos
- En CI/CD, mejor usar solo screenshots on failure

**Recomendaciones:**

| Escenario | Video Enabled |
|-----------|---------------|
| **Debugging local** | ✅ |
| **CI/CD (smoke)** | ❌ |
| **CI/CD (nightly)** | ✅ (solo critical tests) |
| **Producción** | ✅ (solo smoke tests) |

### Configuración por Tipo de Test

```bash
# Para API tests (rápidos, sin video)
RUN_UI_TESTS=false
SCREENSHOT_ON_FAILURE=true
PLAYWRIGHT_VIDEO_ENABLED=false

# Para UI tests (debugging completo)
RUN_UI_TESTS=true
SCREENSHOT_BEFORE_STEP=true
SCREENSHOT_AFTER_STEP=true
SCREENSHOT_ON_FAILURE=true
PLAYWRIGHT_VIDEO_ENABLED=true

# Para CI/CD (performance optimizado)
RUN_UI_TESTS=true
SCREENSHOT_BEFORE_STEP=false
SCREENSHOT_AFTER_STEP=false
SCREENSHOT_ON_FAILURE=true
PLAYWRIGHT_VIDEO_ENABLED=false
```

---

## Estructura de Resultados

### Directorio de Resultados

```
Tests/bin/Debug/net8.0/Reports/AllureResults/
├── <uuid>-result.json          # Resultado de cada test
├── <uuid>-container.json       # Metadata de suites
├── <uuid>-attachment.png       # Screenshots
├── <uuid>-attachment.txt       # Logs
├── videos/
│   └── <scenario>-<timestamp>.webm
└── categories.json             # Clasificación de errores
```

### Archivos Generados

| Archivo | Contenido |
|---------|-----------|
| `*-result.json` | Resultado del test (pass/fail, timing, steps) |
| `*-container.json` | Metadata de features/scenarios |
| `*-attachment.*` | Screenshots, videos, JSON, logs |
| `categories.json` | Clasificación de errores |
| `environment.properties` | Información del entorno de ejecución |

---

## Buenas Prácticas

### 1. Limpieza de Resultados Antiguos

**Automático (ya configurado):**

```csharp
// allureConfig.json
{
  "allure": {
    "cleanupCycle": 1  // Limpia en cada ejecución
  }
}
```

**Manual:**

```powershell
# Limpiar resultados antiguos
Remove-Item "Tests/bin/Debug/net8.0/Reports/AllureResults/*" -Recurse -Force

# Limpiar reportes generados
Remove-Item "Reports/AllureReport/*" -Recurse -Force
```

### 2. Retención de Reportes

**Local:**
- Mantener solo últimos 7 días de reportes
- Usar script de limpieza semanal

**CI/CD:**

```yaml
# GitHub Actions
retention-days: 30  # Ajustar según necesidades

# Azure DevOps
RetentionDays: 30

# Jenkins
numToKeepStr: '10'  # Mantener últimos 10 builds
```

### 3. Categorización de Errores

El framework ya incluye categorías en `allureConfig.json`:

```json
{
  "categories": [
    {
      "name": "Product Bugs",
      "matchedStatuses": ["failed"],
      "messageRegex": ".*AssertionException.*"
    },
    {
      "name": "Test Infrastructure Issues",
      "matchedStatuses": ["broken"],
      "messageRegex": ".*(TimeoutException|WebException|PlaywrightException).*"
    }
  ]
}
```

**Personalizar categorías:**

```json
{
  "name": "Login Issues",
  "matchedStatuses": ["failed"],
  "messageRegex": ".*(Authentication|Login|Unauthorized).*"
}
```

### 4. Historial de Trends

**El script `allure-report.ps1` ya maneja esto automáticamente.**

Para ver tendencias históricas en tu reporte:

1. **Primera ejecución:**
   ```powershell
   dotnet test --filter "Category=unit"
   ./scripts/allure-report.ps1
   ```
   Resultado: Sin tendencias aún (solo 1 ejecución)

2. **Segunda ejecución:**
   ```powershell
   dotnet test --filter "Category=unit"
   ./scripts/allure-report.ps1
   ```
   Resultado: ¡Aparecen las tendencias! (mínimo 2 ejecuciones)

3. **Ejecuciones subsiguientes:**
   Cada vez que ejecutes tests y generes el reporte, las tendencias se acumularán.

**¿Cómo funciona?**
El script automáticamente:
1. Copia el directorio `history` del reporte anterior
2. Lo coloca en `AllureResults/history` antes de generar el nuevo reporte
3. Allure lee este historial y genera gráficos de tendencias

**Manualmente (si usas comandos de Allure directos):**
```powershell
# Copiar history del reporte anterior
Copy-Item "Reports/AllureReport/history" -Destination "Tests/bin/Debug/net8.0/Reports/AllureResults/history" -Recurse -Force

# Generar nuevo reporte (conservará trends)
allure generate Tests/bin/Debug/net8.0/Reports/AllureResults -o Reports/AllureReport --clean
```

**Limpiar historial (empezar de cero):**
```powershell
# Borrar history del reporte
Remove-Item "Reports/AllureReport/history" -Recurse -Force -ErrorAction SilentlyContinue

# Borrar history de resultados
Remove-Item "Tests/bin/Debug/net8.0/Reports/AllureResults/history" -Recurse -Force -ErrorAction SilentlyContinue
```

**En CI/CD (ya configurado en GitHub Actions):**

```yaml
- name: Get Allure history
  uses: actions/checkout@v4
  with:
    ref: gh-pages
    path: gh-pages

- name: Allure Report with history
  uses: simple-elly/allure-report-action@v1
  with:
    allure_history: gh-pages/allure-history
```

### 5. Performance

**Optimizar generación:**

```powershell
# Usar --clean para regenerar desde cero
allure generate <results-dir> -o <output-dir> --clean

# Sin --clean (más rápido, pero puede tener datos viejos)
allure generate <results-dir> -o <output-dir>
```

**Reducir tamaño de reportes:**
- Deshabilitar videos en CI/CD
- Capturar screenshots solo on failure
- Limpiar resultados antiguos regularmente

---

## Troubleshooting

### "Could not find allure-results directory"

```powershell
# Verificar que existe el directorio
Test-Path "Tests/bin/Debug/net8.0/Reports/AllureResults"

# Si no existe, ejecutar tests primero
dotnet test
```

### "Allure command not found"

```powershell
# Verificar instalación
allure --version

# Si no está instalado
winget install QA.Allure

# Reiniciar terminal después de instalar
```

### "Report is empty / No tests shown"

```powershell
# Verificar que hay archivos *-result.json
Get-ChildItem "Tests/bin/Debug/net8.0/Reports/AllureResults" -Filter "*-result.json"

# Si no hay, ejecutar tests primero
dotnet test

# Verificar que Allure está configurado en tests
# Debe tener [AllureNUnit] o [AllureBefore]/[AllureAfter] attributes
```

### "Screenshots not attached"

```bash
# Verificar configuración en .env
SCREENSHOT_ON_FAILURE=true

# Verificar que tests usan tag @ui
# Solo tests UI capturan screenshots
```

### "Videos not appearing"

```bash
# 1. Habilitar videos
PLAYWRIGHT_VIDEO_ENABLED=true

# 2. Verificar que test es @ui
# Solo tests UI graban video

# 3. Ejecutar tests
dotnet test --filter "Category=ui"

# 4. Verificar directorio de videos
Get-ChildItem "Tests/bin/Debug/net8.0/Reports/AllureResults" -Filter "*.webm"
```

### "Trends not showing historical data"

```powershell
# Copiar history del reporte anterior
Copy-Item "Reports/AllureReport/history" -Destination "Tests/bin/Debug/net8.0/Reports/AllureResults/history" -Recurse -Force

# Regenerar reporte
allure generate Tests/bin/Debug/net8.0/Reports/AllureResults -o Reports/AllureReport --clean
```

---

## Comandos de Referencia Rápida

```powershell
# Ejecutar tests y generar reporte (todo-en-uno)
dotnet test; ./scripts/allure-report.ps1 -Open

# Solo ejecutar tests
dotnet test

# Solo generar reporte
allure generate Tests/bin/Debug/net8.0/Reports/AllureResults -o Reports/AllureReport --clean

# Abrir reporte existente
allure open Reports/AllureReport

# Servir reporte en puerto específico
allure open Reports/AllureReport -p 8080

# Limpiar todo
Remove-Item "Tests/bin/Debug/net8.0/Reports/AllureResults/*" -Recurse -Force
Remove-Item "Reports/AllureReport/*" -Recurse -Force
```

---

## Recursos Adicionales

- [Allure Documentation](https://docs.qameta.io/allure/)
- [Allure NUnit Adapter](https://github.com/allure-framework/allure-csharp)
- [Allure Report GitHub Action](https://github.com/simple-elly/allure-report-action)

---

**¿Tienes problemas?** Consulta [RUNBOOK.md](RUNBOOK.md) o abre un issue en el repositorio.
