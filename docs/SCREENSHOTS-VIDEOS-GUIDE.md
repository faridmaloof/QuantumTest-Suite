# Screenshots y Videos - Guía de Configuración

## 📸 Configuración de Screenshots

### Opciones Disponibles

Los screenshots se configuran en `appsettings.json` o mediante variables de entorno:

```json
{
  "Playwright": {
    "ScreenshotOptions": {
      "BeforeStep": true,   // Captura ANTES de cada paso
      "AfterStep": true,    // Captura DESPUÉS de cada paso
      "OnFailure": true     // Captura cuando falla un test
    }
  }
}
```

### Variables de Entorno (.env)

```bash
SCREENSHOT_BEFORE_STEP=false
SCREENSHOT_AFTER_STEP=false
SCREENSHOT_ON_FAILURE=true
```

### Cuándo Usar Cada Opción

| Opción | Uso Recomendado | Volumen | Descripción |
|--------|----------------|---------|-------------|
| **BeforeStep** | Debugging local | 🔴 Alto | Captura screenshot antes de ejecutar cada paso. Útil para ver el estado inicial. |
| **AfterStep** | Debugging local | 🔴 Alto | Captura screenshot después de cada paso. Útil para ver el resultado de cada acción. |
| **OnFailure** | CI/CD, Producción | 🟢 Bajo | Solo captura cuando un test falla. **RECOMENDADO** para pipelines. |

### Configuraciones Recomendadas

#### 🏠 Desarrollo Local (Debugging)
```bash
SCREENSHOT_BEFORE_STEP=true
SCREENSHOT_AFTER_STEP=true
SCREENSHOT_ON_FAILURE=true
```
- ✅ Máxima visibilidad
- ❌ Genera muchos archivos
- ⚠️ Ralentiza la ejecución

#### 🚀 CI/CD / Pipeline
```bash
SCREENSHOT_BEFORE_STEP=false
SCREENSHOT_AFTER_STEP=false
SCREENSHOT_ON_FAILURE=true
```
- ✅ Solo captura errores
- ✅ Ejecución rápida
- ✅ Mínimo uso de almacenamiento

#### 🧪 Testing Manual
```bash
SCREENSHOT_BEFORE_STEP=false
SCREENSHOT_AFTER_STEP=true
SCREENSHOT_ON_FAILURE=true
```
- ⚖️ Balance entre detalle y velocidad
- ✅ Útil para documentar flujos

---

## 🎥 Configuración de Videos

### Activar Video Recording

#### En appsettings.json:
```json
{
  "Playwright": {
    "VideoEnabled": true
  }
}
```

#### En .env:
```bash
PLAYWRIGHT_VIDEO_ENABLED=true
```

### Ubicación de Videos

Los videos se guardan en:
```
Reports/AllureResults/videos/
```

### Cómo Funciona

1. **Activación**: Cuando `VideoEnabled=true`, Playwright graba todo el escenario
2. **Captura**: El video captura TODAS las interacciones con el navegador
3. **Adjuntar**: Al finalizar el test, el video se adjunta automáticamente al reporte Allure
4. **Limpieza**: Los videos se archivan con el reporte Allure

### Configuración en Código

El sistema está configurado en:

**ContextFactory.cs**:
```csharp
if (settings.Playwright.VideoEnabled)
{
    options.RecordVideoDir = Path.Combine(settings.Allure.Directory, "videos");
    options.RecordVideoSize = new RecordVideoSize { Width = 1920, Height = 1080 };
}
```

**TestHooks.cs** (AfterScenario):
```csharp
// Attach video if recording was enabled
if (_settings.Playwright.VideoEnabled && context.Page?.Video != null)
{
    await AllureHelper.AttachVideoAsync(context.Page);
}
```

### Cuándo Usar Videos

| Escenario | Recomendación | Razón |
|-----------|---------------|-------|
| **Desarrollo Local** | ❌ NO | Ocupa espacio, ralentiza tests |
| **CI/CD Failed Tests** | ✅ SÍ | Ayuda a diagnosticar fallos en pipeline |
| **Testing Exploratorio** | ✅ SÍ | Útil para documentar bugs |
| **Tests de Regresión** | ⚠️ OPCIONAL | Solo si hay fallos intermitentes |
| **Tests Unitarios** | ❌ NO | No aplica (sin UI) |

### Configuraciones Recomendadas

#### 🏠 Desarrollo Local
```bash
PLAYWRIGHT_VIDEO_ENABLED=false
SCREENSHOT_ON_FAILURE=true
```
- ✅ Rápido
- ✅ Suficiente detalle con screenshots

#### 🚀 CI/CD
```bash
PLAYWRIGHT_VIDEO_ENABLED=true
SCREENSHOT_ON_FAILURE=true
```
- ✅ Máximo detalle para diagnóstico
- ✅ Útil para investigar fallos en pipeline

#### 🧪 Tests de Smoke
```bash
PLAYWRIGHT_VIDEO_ENABLED=false
SCREENSHOT_ON_FAILURE=true
```
- ✅ Ejecución rápida
- ✅ Captura solo fallos

---

## 📊 Impacto en Performance

### Screenshots

| Configuración | Screenshots por Test | Tiempo Adicional | Espacio en Disco |
|---------------|---------------------|------------------|------------------|
| OnFailure only | 0-1 | ~0-200ms | ~50-200KB |
| AfterStep | 5-10 | ~500ms-2s | ~500KB-2MB |
| Before+After | 10-20 | ~1-3s | ~1-4MB |

### Videos

| Configuración | Videos por Test | Tiempo Adicional | Espacio en Disco |
|---------------|----------------|------------------|------------------|
| Disabled | 0 | 0ms | 0MB |
| Enabled (30s test) | 1 | ~100-300ms | ~2-5MB |
| Enabled (2min test) | 1 | ~200-500ms | ~8-20MB |

---

## 🔍 Verificación de Configuración

### 1. Verificar que Screenshots funcionan

```powershell
# Habilitar todas las opciones
$env:SCREENSHOT_BEFORE_STEP="true"
$env:SCREENSHOT_AFTER_STEP="true"
$env:SCREENSHOT_ON_FAILURE="true"

# Ejecutar un test UI simple
dotnet test --filter "FullyQualifiedName~LoginExitoso"

# Ver el reporte
./scripts/allure-report.ps1 -Open
```

**Qué buscar en Allure**:
- ✅ Attachment "screenshot-before-{step}"
- ✅ Attachment "screenshot-after-{step}"
- ✅ Attachment "screenshot-failure-{step}" (si falla)

### 2. Verificar que Videos funcionan

```powershell
# Habilitar videos
$env:PLAYWRIGHT_VIDEO_ENABLED="true"

# Ejecutar un test UI
dotnet test --filter "Category=ui" --filter "FullyQualifiedName~LoginExitoso"

# Verificar que existe el archivo de video
Get-ChildItem "Reports/AllureResults/videos/" -Filter "*.webm"

# Ver el reporte (debe tener attachment de video)
./scripts/allure-report.ps1 -Open
```

**Qué buscar en Allure**:
- ✅ Attachment con nombre "video" o "recording"
- ✅ Formato .webm
- ✅ Reproducible en el navegador

---

## 🐛 Troubleshooting

### Screenshots no aparecen en el reporte

**Problema**: Los hooks BeforeStep/AfterStep no se ejecutan

**Solución**:
1. Verificar que el scenario tiene la etiqueta `@ui`:
   ```gherkin
   @ui
   Scenario: Login exitoso
   ```

2. Verificar que el hook está registrado:
   ```csharp
   [BeforeStep("ui")]
   public async Task CaptureScreenshotBeforeStep()
   ```

3. Verificar que la configuración está cargada:
   ```csharp
   // En TestHooks.cs debe existir:
   AllureHelper.Initialize(settings);
   ```

### Videos no se adjuntan

**Problema**: Video se graba pero no aparece en Allure

**Soluciones**:
1. Verificar que `VideoEnabled=true` en appsettings.json
2. Verificar que existe el hook AfterScenario:
   ```csharp
   [AfterScenario("ui")]
   public async Task CleanupUiAsync()
   ```
3. Verificar que la carpeta existe:
   ```powershell
   New-Item -ItemType Directory -Force -Path "Reports/AllureResults/videos"
   ```

### Screenshots solo capturan en OnFailure

**Problema**: BeforeStep y AfterStep configurados pero no funcionan

**Diagnóstico**:
1. Los tests fallan antes de ejecutar pasos → No hay página cargada
2. Configuración de URLs vacía → Playwright no puede navegar

**Solución**:
```bash
# En .env:
PLAYWRIGHT_BASE_URL=https://www.saucedemo.com
SAUCEDEMO_USERNAME=standard_user
SAUCEDEMO_PASSWORD=secret_sauce
```

---

## 📝 Ejemplo Completo

### Configuración Óptima para CI/CD

**.env**:
```bash
# Playwright
PLAYWRIGHT_HEADLESS=true
PLAYWRIGHT_VIDEO_ENABLED=true
PLAYWRIGHT_BASE_URL=https://www.saucedemo.com

# Screenshots: Solo fallos
SCREENSHOT_BEFORE_STEP=false
SCREENSHOT_AFTER_STEP=false
SCREENSHOT_ON_FAILURE=true

# APIs
API_HTTPBIN=https://httpbin.org
API_RESTFUL_BOOKER=https://restful-booker.herokuapp.com
API_GITHUB=https://api.github.com

# Credenciales
SAUCEDEMO_USERNAME=standard_user
SAUCEDEMO_PASSWORD=secret_sauce
```

### Ejecución:

```powershell
# Ejecutar tests UI
dotnet test --filter "Category=ui"

# Generar reporte
./scripts/allure-report.ps1 -Open
```

### Resultado Esperado:

- ✅ Videos de todos los tests UI (éxitos y fallos)
- ✅ Screenshots solo de tests fallidos
- ✅ Reporte Allure con evidencias completas
- ✅ Tiempo de ejecución optimizado

---

## 🎯 Mejores Prácticas

1. **CI/CD**: Video=ON, Screenshots=OnFailure
2. **Local Development**: Video=OFF, Screenshots=AfterStep (opcional)
3. **Debugging**: Video=ON, Screenshots=All
4. **Performance Tests**: Video=OFF, Screenshots=OFF
5. **Smoke Tests**: Video=OFF, Screenshots=OnFailure

---

## 📚 Referencias

- [Playwright Video Recording](https://playwright.dev/dotnet/docs/videos)
- [Playwright Screenshots](https://playwright.dev/dotnet/docs/screenshots)
- [Allure Attachments](https://docs.qameta.io/allure/#_attachments)
