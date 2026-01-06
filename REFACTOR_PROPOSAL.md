# 📋 Propuesta de Refactorización - Estructura de Directorios

**Fecha**: 6 de enero de 2026  
**Estado**: 🔴 REQUIERE MEJORA  
**Razón**: Estructura actual es confusa y difícil de navegar

---

## 🔍 Problema Actual

### Estructura Existente (Confusa)

```
Tests/
├── API/                    # ❌ Clientes API mezclados con tests
│   ├── Clients/
│   ├── Endpoints/
│   ├── Helpers/
│   ├── Models/
│   └── Questions/
├── Core/                   # ❌ Core framework mezclado con tests
│   ├── Configuration/
│   ├── Context/
│   ├── DependencyInjection/
│   ├── Drivers/
│   ├── Models/
│   ├── Services/
│   └── Utilities/
├── Data/                   # ❌ Test data mezclado con tests
│   └── Factories/
├── Features/               # ✅ CORRECTO - Feature files
│   ├── ApiFeatures/
│   ├── UiFeatures/
│   └── UnitFeatures/
├── Tests/                  # ❌ CONFUSO - ¿Por qué "Tests/Tests"?
│   └── StepBindings/
│       ├── Api/
│       ├── Base/
│       ├── Ui/
│       └── Unit/
├── UI/                     # ❌ UI framework mezclado con tests
│   ├── Drivers/
│   ├── Locators/
│   ├── Pages/
│   └── Screenplay/
│       ├── Abilities/
│       ├── Actors/
│       ├── Questions/
│       └── Tasks/
└── Reports/                # ✅ CORRECTO - Test outputs
```

### ❌ Problemas Identificados

1. **Nombre Confuso**: `Tests/Tests/` - duplicación innecesaria
2. **Mezclado de Responsabilidades**:
   - Código de framework (Core, UI, API) dentro de carpeta Tests
   - Dificulta distinguir entre "infraestructura" vs "pruebas"
3. **Navegación Confusa**:
   - ¿Dónde están los Page Objects? → `Tests/UI/Pages/`
   - ¿Dónde están los Step Bindings? → `Tests/Tests/StepBindings/`
   - No hay lógica clara
4. **No escala bien**:
   - Si el proyecto crece, será caótico
   - Difícil onboarding para nuevos desarrolladores

---

## ✅ Propuesta de Mejora

### Opción 1: Estructura por Capas (RECOMENDADA) 🎯

Separar claramente **Framework** vs **Tests**

```
QuantumTestSuite/
│
├── src/                                    # 🆕 FRAMEWORK (reutilizable)
│   ├── QuantumTestSuite.Core/              # Core framework
│   │   ├── Configuration/
│   │   ├── DependencyInjection/
│   │   ├── Models/
│   │   ├── Services/
│   │   └── Utilities/
│   │
│   ├── QuantumTestSuite.API/               # API automation framework
│   │   ├── Clients/
│   │   ├── Endpoints/
│   │   ├── Helpers/
│   │   ├── Models/
│   │   └── Questions/
│   │
│   └── QuantumTestSuite.UI/                # UI automation framework
│       ├── Drivers/
│       ├── Locators/
│       ├── Pages/
│       └── Screenplay/
│           ├── Abilities/
│           ├── Actors/
│           ├── Questions/
│           └── Tasks/
│
└── tests/                                  # 🆕 TESTS ONLY
    ├── Features/                           # Gherkin scenarios
    │   ├── API/
    │   ├── UI/
    │   └── Unit/
    │
    ├── StepBindings/                       # ✅ NO MÁS "Tests/Tests"
    │   ├── API/
    │   ├── UI/
    │   ├── Unit/
    │   └── Base/
    │
    ├── TestData/                           # Test-specific data
    │   └── Factories/
    │
    ├── Hooks/                              # Test hooks
    │   └── TestHooks.cs
    │
    └── Configuration/                      # Test-specific config
        ├── appsettings.json
        ├── reqnroll.json
        └── allureConfig.json
```

**✅ Ventajas**:
- ✅ Clara separación: Framework vs Tests
- ✅ Framework puede convertirse en NuGet package
- ✅ Tests solo contiene lo relacionado con pruebas
- ✅ Escalable y profesional
- ✅ Fácil de entender para nuevos desarrolladores

**⚠️ Desventajas**:
- Requiere refactorización significativa
- Cambiar namespaces
- Actualizar referencias en .csproj

---

### Opción 2: Reorganización Mínima (RÁPIDA) ⚡

Mantener todo en `Tests/` pero mejorar nombres

```
Tests/
├── Features/                               # ✅ MANTENER
│   ├── API/
│   ├── UI/
│   └── Unit/
│
├── StepBindings/                           # 🔄 RENOMBRAR "Tests" → "StepBindings"
│   ├── API/
│   ├── UI/
│   ├── Unit/
│   └── Base/
│
├── Framework/                              # 🆕 MOVER Core, API base, UI base aquí
│   ├── API/
│   │   ├── Clients/
│   │   ├── Endpoints/
│   │   ├── Helpers/
│   │   ├── Models/
│   │   └── Questions/
│   ├── Core/
│   │   ├── Configuration/
│   │   ├── DependencyInjection/
│   │   ├── Drivers/
│   │   ├── Models/
│   │   ├── Services/
│   │   └── Utilities/
│   └── UI/
│       ├── Drivers/
│       ├── Locators/
│       ├── Pages/
│       └── Screenplay/
│
├── TestData/                               # 🔄 RENOMBRAR "Data" → "TestData"
│   └── Factories/
│
├── Hooks/                                  # Test lifecycle hooks
│
└── Configuration/                          # Config files
    ├── appsettings.json
    ├── reqnroll.json
    └── allureConfig.json
```

**✅ Ventajas**:
- ✅ Menos cambios que Opción 1
- ✅ Mejora claridad sin refactorización mayor
- ✅ Elimina confusión de "Tests/Tests"
- ✅ Framework agrupado bajo carpeta clara

**⚠️ Desventajas**:
- No separa completamente framework de tests
- No permite fácilmente extraer framework a NuGet

---

### Opción 3: Flat pero Descriptivo (MÁS SIMPLE)

Mantener flat pero con nombres más claros

```
Tests/
├── Features/                               # ✅ MANTENER
├── StepDefinitions/                        # 🔄 "Tests" → "StepDefinitions"
├── PageObjects/                            # 🔄 "UI/Pages" → "PageObjects"
├── Locators/                               # 🔄 "UI/Locators" → "Locators"
├── Screenplay/                             # 🔄 "UI/Screenplay" → "Screenplay"
├── ApiClients/                             # 🔄 "API/Clients" → "ApiClients"
├── ApiModels/                              # 🔄 "API/Models" → "ApiModels"
├── Questions/                              # 🔄 Consolidar UI + API Questions
├── TestData/                               # 🔄 "Data" → "TestData"
├── Core/                                   # ✅ MANTENER (Configuration, DI, etc.)
└── Configuration/                          # Config files
```

**✅ Ventajas**:
- ✅ Mínimos cambios
- ✅ Nombres autoexplicativos
- ✅ Flat = fácil de navegar

**⚠️ Desventajas**:
- Muchas carpetas al mismo nivel
- No agrupa conceptos relacionados
- Puede crecer desordenado

---

## 🎯 Recomendación Final

### Para el estado actual del proyecto: **OPCIÓN 2** ⚡

**Razones**:
1. ✅ **Balance** entre mejora y esfuerzo
2. ✅ **Elimina confusión** de "Tests/Tests"
3. ✅ **Agrupa framework** bajo carpeta clara
4. ✅ **No rompe estructura** de Features/StepBindings
5. ✅ **Preparado para futuro** (puede evolucionar a Opción 1)

### Plan de Migración (Opción 2)

#### Paso 1: Renombrar y Reorganizar
```powershell
# 1. Renombrar Tests/Tests → Tests/StepBindings
Move-Item "Tests/Tests" "Tests/StepBindings_TEMP"

# 2. Crear estructura Framework
New-Item -ItemType Directory -Path "Tests/Framework"
Move-Item "Tests/API" "Tests/Framework/API"
Move-Item "Tests/Core" "Tests/Framework/Core"
Move-Item "Tests/UI" "Tests/Framework/UI"

# 3. Renombrar Data → TestData
Move-Item "Tests/Data" "Tests/TestData"

# 4. Finalizar rename StepBindings
Move-Item "Tests/StepBindings_TEMP" "Tests/StepBindings"
```

#### Paso 2: Actualizar Namespaces
```csharp
// ANTES
namespace QuantumTestSuite.API.Clients
namespace QuantumTestSuite.Core.Services
namespace QuantumTestSuite.UI.Pages
namespace QuantumTestSuite.Tests.StepBindings.Api

// DESPUÉS
namespace QuantumTestSuite.Framework.API.Clients
namespace QuantumTestSuite.Framework.Core.Services
namespace QuantumTestSuite.Framework.UI.Pages
namespace QuantumTestSuite.StepBindings.API
```

#### Paso 3: Actualizar Using Statements
```csharp
// ANTES
using QuantumTestSuite.API.Clients;
using QuantumTestSuite.Core.Services;
using QuantumTestSuite.UI.Pages;

// DESPUÉS
using QuantumTestSuite.Framework.API.Clients;
using QuantumTestSuite.Framework.Core.Services;
using QuantumTestSuite.Framework.UI.Pages;
```

#### Paso 4: Actualizar FILE-STRUCTURE-GUIDE.md
Documentar la nueva estructura en la guía.

#### Paso 5: Ejecutar Tests
```powershell
# Verificar que todo sigue funcionando
dotnet clean
dotnet restore
dotnet build
dotnet test
```

---

## 📊 Comparación de Opciones

| Criterio | Opción 1 (Capas) | Opción 2 (Framework) | Opción 3 (Flat) |
|----------|------------------|---------------------|-----------------|
| **Claridad** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Esfuerzo** | 🔴 Alto | 🟡 Medio | 🟢 Bajo |
| **Escalabilidad** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐ |
| **Onboarding** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Profesionalidad** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Reutilización** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐ |

---

## ✅ Próximos Pasos

### Si eliges Opción 2 (Recomendada):

1. **Backup**:
   ```powershell
   git checkout -b feature/restructure-directories
   git add .
   git commit -m "chore: backup before directory restructure"
   ```

2. **Ejecutar migración**:
   - Seguir "Plan de Migración" paso a paso
   - Commit después de cada paso exitoso

3. **Actualizar documentación**:
   - FILE-STRUCTURE-GUIDE.md
   - ARCHITECTURE.md
   - README.md

4. **Verificar**:
   ```powershell
   dotnet test  # Debe pasar 14/14 tests
   ```

5. **Pull Request**:
   ```powershell
   git push origin feature/restructure-directories
   # Crear PR con descripción de cambios
   ```

---

## 🎓 Para el Equipo

### ¿Por qué reorganizar?

**Antes (Confuso)**:
- "¿Dónde están los Page Objects?" → `Tests/UI/Pages`
- "¿Dónde están los Step Bindings?" → `Tests/Tests/StepBindings` ❌
- "¿Qué es Tests/Tests?" → 🤷 Confuso

**Después (Claro)**:
- "¿Dónde está el framework?" → `Tests/Framework/`
- "¿Dónde están los Step Bindings?" → `Tests/StepBindings/` ✅
- "¿Dónde están las Features?" → `Tests/Features/` ✅

### Impacto en Desarrollo

- ✅ **Onboarding más rápido**: Estructura autoexplicativa
- ✅ **Menos errores**: Ubicaciones claras y predecibles
- ✅ **Mejor mantenimiento**: Conceptos agrupados lógicamente
- ✅ **Escalabilidad**: Preparado para crecimiento

---

**Estado Actual**: 🔴 Requiere Mejora  
**Propuesta**: ✅ Opción 2 (Framework folder)  
**Impacto**: 🟡 Medio (2-3 horas de trabajo)  
**Beneficio**: ⭐⭐⭐⭐⭐ (Claridad permanente)

---

**¿Proceder con la refactorización?** 
Confirma y ejecuto el plan de migración paso a paso.
