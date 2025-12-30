# Guía de Migración a Abilities

Esta guía te ayudará a migrar tu código existente al nuevo sistema de **Abilities** en QuantumTestSuite.

## Tabla de Contenidos
1. [Resumen de Cambios](#resumen-de-cambios)
2. [Estrategia de Migración](#estrategia-de-migración)
3. [Antes vs Después](#antes-vs-después)
4. [Migraciones Comunes](#migraciones-comunes)
5. [Checklist de Migración](#checklist-de-migración)
6. [Casos Especiales](#casos-especiales)
7. [FAQ](#faq)

---

## Resumen de Cambios

### ¿Qué cambió?

| Concepto | Antes (Sin Abilities) | Después (Con Abilities) |
|----------|----------------------|------------------------|
| **Compartir datos** | `ScenarioContext["key"]` + casting | `actor.Using<RememberData>().Remember/Recall<T>()` |
| **Queries DB** | Inyectar `IDbConnection` en step bindings | `actor.Using<AccessDatabase>().Query...()` |
| **API Calls** | Crear HttpClient en steps | `actor.Using<CallApiEndpoint>().Get/Post...()` |
| **Configuración** | `ConfigManager.Settings` directo | `actor.Using<ReadConfiguration>().Get...()` |
| **Cleanup** | Manual en AfterScenario | `actor.CleanupAsync()` automático |
| **Type Safety** | Casting manual, propenso a errores | Genéricos type-safe, compile-time |

### ¿Por qué migrar?

✅ **Type Safety**: Eliminación de magic strings y castings  
✅ **Testabilidad**: Abilities mockables para unit tests  
✅ **Screenplay Puro**: Mantiene la filosofía del patrón  
✅ **Desacoplamiento**: Steps no conocen infraestructura  
✅ **Expresividad**: Código más legible y mantenible  

### Compatibilidad

- ✅ **Código existente sigue funcionando**: Migración gradual posible
- ✅ **No breaking changes**: Actor extiende funcionalidad, no la reemplaza
- ⚠️ **Nuevos scenarios deben usar Abilities**: Best practice recomendada

---

## Estrategia de Migración

### Enfoque Recomendado: **Gradual**

1. **Fase 1**: Nuevos scenarios usan Abilities (100% adoption para código nuevo)
2. **Fase 2**: Migrar scenarios críticos o frecuentemente modificados
3. **Fase 3**: Migrar scenarios legacy cuando toque mantenimiento

### Cronograma Sugerido

| Semana | Actividad |
|--------|-----------|
| 1 | Setup inicial: Crear Abilities en proyecto, documentación del equipo |
| 2 | Migrar 1-2 features críticas, validar funcionamiento |
| 3-4 | Migrar features de alta frecuencia (smoke tests, login, etc.) |
| 5+ | Migración incremental del resto (a demanda) |

---

## Antes vs Después

### Escenario 1: Compartir Datos entre Steps

#### ❌ ANTES (ScenarioContext)

```csharp
[Binding]
public class LoginStepBindings
{
    private readonly ScenarioContext _scenarioContext;
    
    public LoginStepBindings(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }
    
    [Given(@"tengo un usuario de prueba")]
    public void TengoUsuarioPrueba()
    {
        var user = new User
        {
            Username = "testuser",
            Password = "password123",
            Email = "test@example.com"
        };
        
        // ❌ Magic string, sin type safety
        _scenarioContext["currentUser"] = user;
    }
    
    [When(@"inicio sesión")]
    public async Task InicioSesion()
    {
        // ❌ Casting manual, propenso a errores
        var user = (User)_scenarioContext["currentUser"];
        
        await _actor.AttemptsTo(
            Login.WithCredentials(user.Username, user.Password)
        );
    }
    
    [Then(@"verifico el nombre mostrado")]
    public void VerificoNombre()
    {
        // ❌ Casting repetido
        var user = (User)_scenarioContext["currentUser"];
        
        _actor.AttemptsTo(
            Verify.That(UserPage.DisplayName, Is.EqualTo(user.FullName))
        );
    }
}
```

#### ✅ DESPUÉS (RememberData Ability)

```csharp
[Binding]
public class LoginStepBindings
{
    private readonly Actor _actor;
    
    public LoginStepBindings(Actor actor)
    {
        _actor = actor;
    }
    
    [Given(@"tengo un usuario de prueba")]
    public void TengoUsuarioPrueba()
    {
        var user = new User
        {
            Username = "testuser",
            Password = "password123",
            Email = "test@example.com"
        };
        
        // ✅ Type-safe, expresivo
        _actor.Using<RememberData>().Remember("currentUser", user);
    }
    
    [When(@"inicio sesión")]
    public async Task InicioSesion()
    {
        // ✅ Type-safe, sin casting
        var user = _actor.Using<RememberData>().Recall<User>("currentUser");
        
        await _actor.AttemptsTo(
            Login.WithCredentials(user.Username, user.Password)
        );
    }
    
    [Then(@"verifico el nombre mostrado")]
    public void VerificoNombre()
    {
        // ✅ Type-safe
        var user = _actor.Using<RememberData>().Recall<User>("currentUser");
        
        _actor.AttemptsTo(
            Verify.That(UserPage.DisplayName, Is.EqualTo(user.FullName))
        );
    }
}
```

**Migración**:
1. Cambiar `_scenarioContext["key"] = value` → `_actor.Using<RememberData>().Remember("key", value)`
2. Cambiar `(Type)_scenarioContext["key"]` → `_actor.Using<RememberData>().Recall<Type>("key")`
3. Asignar `RememberData` ability en BeforeScenario

---

### Escenario 2: Queries a Base de Datos

#### ❌ ANTES (IDbConnection directa)

```csharp
[Binding]
public class UserManagementStepBindings
{
    private readonly IDbConnection _dbConnection;
    private readonly ScenarioContext _scenarioContext;
    
    public UserManagementStepBindings(
        IDbConnection dbConnection,
        ScenarioContext scenarioContext)
    {
        _dbConnection = dbConnection;
        _scenarioContext = scenarioContext;
    }
    
    [Given(@"tengo un usuario activo en la base de datos")]
    public async Task TengoUsuarioActivoDb()
    {
        // ❌ Acoplamiento directo a Dapper/DB
        // ❌ No hay retry logic
        // ❌ No hay metadata (timing, row count)
        var user = await _dbConnection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Active = 1"
        );
        
        Assert.That(user, Is.Not.Null);
        
        // ❌ ScenarioContext con magic string
        _scenarioContext["dbUser"] = user;
    }
    
    [When(@"actualizo el email del usuario")]
    public async Task ActualizoEmail()
    {
        var user = (User)_scenarioContext["dbUser"];
        var newEmail = "updated@example.com";
        
        // ❌ Query SQL directa en step binding
        await _dbConnection.ExecuteAsync(
            "UPDATE Users SET Email = @Email WHERE Id = @Id",
            new { Email = newEmail, Id = user.Id }
        );
        
        user.Email = newEmail;
        _scenarioContext["dbUser"] = user;
    }
}
```

#### ✅ DESPUÉS (AccessDatabase Ability)

```csharp
[Binding]
public class UserManagementStepBindings
{
    private readonly Actor _actor;
    
    public UserManagementStepBindings(Actor actor)
    {
        _actor = actor;
    }
    
    [Given(@"tengo un usuario activo en la base de datos")]
    public async Task TengoUsuarioActivoDb()
    {
        // ✅ Desacoplado de DB implementation
        // ✅ Retry logic incluido
        // ✅ Metadata tracking (timing, row count)
        var users = await _actor.Using<AccessDatabase>()
            .GetActiveUsersAsync();
        
        Assert.That(users, Is.Not.Empty);
        
        // ✅ Type-safe storage
        _actor.Using<RememberData>().Remember("dbUser", users.First());
    }
    
    [When(@"actualizo el email del usuario")]
    public async Task ActualizoEmail()
    {
        // ✅ Type-safe recall
        var user = _actor.Using<RememberData>().Recall<User>("dbUser");
        var newEmail = "updated@example.com";
        
        // ✅ Query con retry + metadata
        var rowsAffected = await _actor.Using<AccessDatabase>().ExecuteAsync(
            "UPDATE Users SET Email = @Email WHERE Id = @Id",
            new { Email = newEmail, Id = user.Id }
        );
        
        Assert.That(rowsAffected, Is.EqualTo(1));
        
        user.Email = newEmail;
        _actor.Using<RememberData>().Remember("dbUser", user);
    }
}
```

**Migración**:
1. Reemplazar inyección de `IDbConnection` con `Actor` en constructor
2. Cambiar `_dbConnection.QueryAsync<T>(...)` → `_actor.Using<AccessDatabase>().QueryAsync<T>(...)`
3. Usar helpers específicos cuando aplique: `GetActiveUsersAsync()`, `GetUserByIdAsync()`, etc.
4. Almacenar resultados en `RememberData` en lugar de `ScenarioContext`
5. Asignar `AccessDatabase` ability en BeforeScenario

---

### Escenario 3: API Calls

#### ❌ ANTES (HttpClient directo)

```csharp
[Binding]
public class ApiStepBindings
{
    private readonly HttpClient _httpClient;
    private readonly ScenarioContext _scenarioContext;
    
    public ApiStepBindings(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://restful-booker.herokuapp.com")
        };
    }
    
    [When(@"creo un booking vía API")]
    public async Task CreoBookingViaApi()
    {
        var booking = new BookingRequest
        {
            FirstName = "John",
            LastName = "Doe",
            CheckIn = "2024-01-15",
            CheckOut = "2024-01-20"
        };
        
        // ❌ HttpClient directo, sin retry, sin tracking
        var json = JsonSerializer.Serialize(booking);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("/booking", content);
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<BookingResponse>(responseBody);
        
        // ❌ ScenarioContext
        _scenarioContext["bookingId"] = result.BookingId;
    }
}
```

#### ✅ DESPUÉS (CallApiEndpoint Ability)

```csharp
[Binding]
public class ApiStepBindings
{
    private readonly Actor _actor;
    
    public ApiStepBindings(Actor actor)
    {
        _actor = actor;
    }
    
    [When(@"creo un booking vía API")]
    public async Task CreoBookingViaApi()
    {
        var booking = new BookingRequest
        {
            FirstName = "John",
            LastName = "Doe",
            CheckIn = "2024-01-15",
            CheckOut = "2024-01-20"
        };
        
        // ✅ Expresivo, type-safe, con tracking
        var response = await _actor.Using<CallApiEndpoint>()
            .PostAsync<BookingRequest, BookingResponse>(
                "/booking",
                booking,
                "https://restful-booker.herokuapp.com"
            );
        
        Assert.That(response, Is.Not.Null);
        
        // ✅ Type-safe storage
        _actor.Using<RememberData>().Remember("bookingId", response.BookingId);
    }
}
```

**Migración**:
1. Reemplazar `HttpClient` con `CallApiEndpoint` ability
2. Cambiar `_httpClient.GetAsync(...)` → `_actor.Using<CallApiEndpoint>().GetAsync<T>(...)`
3. Cambiar `_httpClient.PostAsync(...)` → `_actor.Using<CallApiEndpoint>().PostAsync<TReq, TRes>(...)`
4. Eliminar serialización/deserialización manual (incluida en ability)
5. Asignar `CallApiEndpoint` ability en BeforeScenario

---

### Escenario 4: Acceso a Configuración

#### ❌ ANTES (ConfigManager directo)

```csharp
[Binding]
public class NavigationStepBindings
{
    [Given(@"navego a la página principal")]
    public async Task NavegoHome()
    {
        // ❌ Acoplamiento directo a ConfigManager
        var baseUrl = ConfigManager.Settings.BaseUrl;
        
        await _actor.AttemptsTo(Navigate.To(baseUrl));
    }
    
    [When(@"inicio sesión como administrador")]
    public async Task InicioSesionAdmin()
    {
        // ❌ Configuración hardcoded o acoplada
        var adminUser = ConfigManager.Settings.AdminUsername;
        var adminPass = ConfigManager.Settings.AdminPassword;
        
        await _actor.AttemptsTo(
            Login.WithCredentials(adminUser, adminPass)
        );
    }
}
```

#### ✅ DESPUÉS (ReadConfiguration Ability)

```csharp
[Binding]
public class NavigationStepBindings
{
    private readonly Actor _actor;
    
    public NavigationStepBindings(Actor actor)
    {
        _actor = actor;
    }
    
    [Given(@"navego a la página principal")]
    public async Task NavegoHome()
    {
        // ✅ Desacoplado, testeable
        var baseUrl = _actor.Using<ReadConfiguration>().GetBaseUrl();
        
        await _actor.AttemptsTo(Navigate.To(baseUrl));
    }
    
    [When(@"inicio sesión como administrador")]
    public async Task InicioSesionAdmin()
    {
        // ✅ Expresivo, centralizado
        var (username, password) = _actor.Using<ReadConfiguration>()
            .GetCredentials("admin");
        
        await _actor.AttemptsTo(
            Login.WithCredentials(username, password)
        );
    }
}
```

**Migración**:
1. Cambiar `ConfigManager.Settings.Property` → `_actor.Using<ReadConfiguration>().GetProperty()`
2. Usar helpers específicos: `GetBaseUrl()`, `GetCredentials()`, `GetApiUrl()`
3. Asignar `ReadConfiguration` ability en BeforeScenario

---

## Migraciones Comunes

### 1. Setup en BeforeScenario

#### ❌ ANTES

```csharp
[BeforeScenario]
public async Task Setup()
{
    var playwright = await Playwright.CreateAsync();
    var browser = await playwright.Chromium.LaunchAsync();
    var page = await browser.NewPageAsync();
    
    _actor = new Actor("TestUser", page);
}
```

#### ✅ DESPUÉS

```csharp
[BeforeScenario]
public async Task Setup()
{
    var playwright = await Playwright.CreateAsync();
    var browser = await playwright.Chromium.LaunchAsync();
    var page = await browser.NewPageAsync();
    var apiContext = await playwright.APIRequest.NewContextAsync();
    
    var dbConnection = new SqlConnection(ConfigManager.Settings.ConnectionString);
    var dbConfig = new DatabaseConfig
    {
        ConnectionString = ConfigManager.Settings.ConnectionString,
        Provider = "SqlServer",
        RetryCount = 3,
        RetryDelayMs = 500
    };
    
    var settings = ConfigManager.Settings;
    
    // ✅ Asignar Abilities
    _actor = new Actor("TestUser", page)
        .WhoCan(new RememberData())
        .WhoCan(new AccessDatabase(dbConnection, dbConfig))
        .WhoCan(new CallApiEndpoint(apiContext, settings))
        .WhoCan(new ReadConfiguration(settings));
}
```

### 2. Cleanup en AfterScenario

#### ❌ ANTES

```csharp
[AfterScenario]
public async Task Cleanup()
{
    // Manual cleanup
    await _actor.Page.CloseAsync();
    _dbConnection?.Dispose();
}
```

#### ✅ DESPUÉS

```csharp
[AfterScenario]
public async Task Cleanup()
{
    // ✅ Cleanup automático de Abilities
    if (_actor != null)
        await _actor.CleanupAsync();
    
    // Cleanup de página sigue igual
    await _actor.Page.CloseAsync();
}
```

---

## Checklist de Migración

### Por cada Feature/Scenario

- [ ] **1. Identificar dependencias**
  - [ ] ¿Usa ScenarioContext para compartir datos? → RememberData
  - [ ] ¿Hace queries a DB? → AccessDatabase
  - [ ] ¿Hace llamadas API? → CallApiEndpoint
  - [ ] ¿Accede a ConfigManager directamente? → ReadConfiguration

- [ ] **2. Actualizar BeforeScenario**
  - [ ] Asignar Abilities necesarias con `.WhoCan(...)`
  - [ ] Configurar conexiones/contextos para Abilities

- [ ] **3. Migrar Step Bindings**
  - [ ] Cambiar inyección de dependencias en constructor (usar `Actor`)
  - [ ] Reemplazar `ScenarioContext["key"]` con `actor.Using<RememberData>().Remember/Recall()`
  - [ ] Reemplazar queries DB directas con `actor.Using<AccessDatabase>()...`
  - [ ] Reemplazar HttpClient con `actor.Using<CallApiEndpoint>()...`
  - [ ] Reemplazar `ConfigManager.Settings` con `actor.Using<ReadConfiguration>()...`

- [ ] **4. Actualizar AfterScenario**
  - [ ] Agregar `await actor.CleanupAsync()`

- [ ] **5. Validar**
  - [ ] Ejecutar scenario y verificar funcionamiento
  - [ ] Verificar no hay memory leaks (cleanup correcto)
  - [ ] Code review con el equipo

### Por el Proyecto Completo

- [ ] **Documentación**
  - [ ] Leer [ABILITIES-GUIDE.md](ABILITIES-GUIDE.md)
  - [ ] Revisar [ADR-005](ADRs/005-abilities-pattern.md)
  - [ ] Revisar esta guía de migración

- [ ] **Setup Inicial**
  - [ ] Validar que Abilities existen en proyecto (Tests/UI/Screenplay/Abilities/)
  - [ ] Validar Dapper package instalado (para AccessDatabase)

- [ ] **Migración Gradual**
  - [ ] Priorizar features críticas primero
  - [ ] Migrar en sprints/iteraciones
  - [ ] No forzar todo de una vez

- [ ] **Testing**
  - [ ] Ejecutar smoke tests después de migración
  - [ ] Validar integración con CI/CD
  - [ ] Revisar cobertura de unit tests

---

## Casos Especiales

### Caso 1: Scenario sin Abilities

Si un scenario NO requiere Abilities (ej: simple UI test):

```csharp
// ✅ Válido: Actor sin Abilities funciona igual que antes
_actor = new Actor("TestUser", page);

await _actor.AttemptsTo(
    Navigate.To("https://example.com"),
    Click.On(LoginButton)
);
```

**No es obligatorio usar Abilities en todos los scenarios.**

---

### Caso 2: Mezclar ScenarioContext y Abilities

Si tienes código legacy complejo, puedes mezclar temporalmente:

```csharp
// ⚠️ Transición: mezclar ambos approaches
[When(@"...")]
public async Task MixedApproach()
{
    // Legacy (ScenarioContext)
    var legacyData = (string)_scenarioContext["oldKey"];
    
    // Nuevo (Abilities)
    var user = _actor.Using<RememberData>().Recall<User>("newUser");
    
    // Ambos conviven durante la migración
}
```

**Gradualmente eliminar ScenarioContext a favor de Abilities.**

---

### Caso 3: Multiple Actors

Si usas múltiples Actors en un scenario:

```csharp
[BeforeScenario]
public void SetupMultipleActors()
{
    var settings = ConfigManager.Settings;
    var rememberData = new RememberData(); // ⚠️ Compartir instancia
    
    _adminActor = new Actor("Admin", adminPage)
        .WhoCan(rememberData) // Misma instancia
        .WhoCan(new ReadConfiguration(settings));
    
    _userActor = new Actor("RegularUser", userPage)
        .WhoCan(rememberData) // Misma instancia
        .WhoCan(new ReadConfiguration(settings));
}

// Ambos Actors comparten la misma RememberData
[When(@"el admin crea un usuario")]
public async Task AdminCreaUsuario()
{
    var user = new User { /* ... */ };
    _adminActor.Using<RememberData>().Remember("sharedUser", user);
}

[Then(@"el usuario puede loguearse")]
public async Task UsuarioPuedeLoguear()
{
    // ✅ User Actor puede acceder al dato compartido
    var user = _userActor.Using<RememberData>().Recall<User>("sharedUser");
    // ...
}
```

---

### Caso 4: Custom Abilities Específicas del Proyecto

Si tienes lógica compleja específica de tu dominio:

```csharp
// Crear custom ability
public class ManageOrdersAbility : IAbility
{
    private readonly IOrderService _orderService;
    
    public ManageOrdersAbility(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    public async Task<Order> CreateOrderAsync(OrderRequest request)
    {
        return await _orderService.CreateAsync(request);
    }
    
    public async Task<List<Order>> GetOrdersByUserAsync(int userId)
    {
        return await _orderService.GetByUserIdAsync(userId);
    }
}

// Usar en scenario
_actor = _actor.WhoCan(new ManageOrdersAbility(_orderService));

var order = await _actor.Using<ManageOrdersAbility>()
    .CreateOrderAsync(orderRequest);
```

---

## FAQ

### ¿Debo migrar TODO mi código existente?

**No.** Migración gradual es recomendada:
- Nuevos scenarios: usar Abilities desde el inicio
- Scenarios existentes: migrar cuando toque mantenimiento o si hay bugs

### ¿Puedo seguir usando ScenarioContext?

**Sí, pero no recomendado.** ScenarioContext seguirá funcionando, pero perderás los beneficios de type safety y expresividad de Abilities.

### ¿Qué hago si tengo código compartido entre features?

Usa **static constants** para keys de RememberData:

```csharp
public static class DataKeys
{
    public const string CurrentUser = "currentUser";
    public const string BookingId = "bookingId";
}

// Todas las features usan las mismas keys
_actor.Using<RememberData>().Remember(DataKeys.CurrentUser, user);
```

### ¿Las Abilities afectan performance?

**No significativamente.** El overhead de las Abilities es mínimo (Dictionary lookup), y los beneficios (retry logic, metadata tracking) superan el costo.

### ¿Cómo testeo step bindings con Abilities?

Mock las Abilities en unit tests:

```csharp
[Test]
public async Task LoginStep_UsesRememberDataCorrectly()
{
    // Arrange
    var mockRememberData = new Mock<RememberData>();
    var user = new User { Username = "test", Password = "pass" };
    mockRememberData.Setup(r => r.Recall<User>("currentUser")).Returns(user);
    
    var actor = new Actor("TestUser", mockPage)
        .WhoCan(mockRememberData.Object);
    
    var stepBinding = new LoginStepBindings(actor);
    
    // Act
    await stepBinding.InicioSesion();
    
    // Assert
    mockRememberData.Verify(r => r.Recall<User>("currentUser"), Times.Once);
}
```

### ¿Debo crear Abilities para TODO?

**No.** Usa Abilities cuando:
- Necesitas compartir estado entre steps (RememberData)
- Interactúas con DB (AccessDatabase)
- Haces API calls (CallApiEndpoint)
- Accedes a configuración frecuentemente (ReadConfiguration)

No uses Abilities para:
- Simple UI interactions (usa Tasks/Interactions directamente)
- Lógica específica de un solo step

### ¿Cómo manejo secrets/passwords en Abilities?

Usa **User Secrets** o **Environment Variables**:

```csharp
// appsettings.json (placeholder)
{
  "ConnectionString": "{{CONNECTION_STRING}}"
}

// User Secrets (local dev)
dotnet user-secrets set "ConnectionString" "Server=...;User=sa;Password=Secret123"

// CI/CD (environment variable)
export CONNECTION_STRING="Server=...;User=sa;Password=SecretFromVault"

// Ability lee de configuración
var connString = ConfigManager.Settings.ConnectionString; // Resuelto automáticamente
_actor.WhoCan(new AccessDatabase(new SqlConnection(connString), config));
```

---

## Soporte

Si tienes dudas durante la migración:

1. **Documentación**:
   - [ABILITIES-GUIDE.md](ABILITIES-GUIDE.md) - Guía completa de uso
   - [ADR-005](ADRs/005-abilities-pattern.md) - Decisión arquitectónica

2. **Ejemplos**:
   - [AbilitiesDemo.feature](../Tests/Features/UiFeatures/AbilitiesDemo.feature) - Scenarios de demostración
   - [AbilitiesDemoStepBindings.cs](../Tests/Tests/StepBindings/AbilitiesDemoStepBindings.cs) - Step bindings de ejemplo

3. **Testing**:
   - [AbilitiesTests.feature](../Tests/Features/UnitFeatures/AbilitiesTests.feature) - Unit tests del sistema

4. **Comunidad**:
   - Slack: #quantum-test-suite
   - Email: qa-team@evertec.com

---

**Última Actualización**: 2024-01-15  
**Mantenido por**: QuantumTestSuite Team
