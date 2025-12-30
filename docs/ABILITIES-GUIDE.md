# Guía de Abilities en QuantumTestSuite

## Tabla de Contenidos
1. [Introducción](#introducción)
2. [Conceptos Fundamentales](#conceptos-fundamentales)
3. [Abilities Disponibles](#abilities-disponibles)
4. [Guía de Uso](#guía-de-uso)
5. [Ejemplos Prácticos](#ejemplos-prácticos)
6. [Crear Custom Abilities](#crear-custom-abilities)
7. [Best Practices](#best-practices)
8. [Troubleshooting](#troubleshooting)

---

## Introducción

El sistema de **Abilities** permite que un **Actor** del Screenplay Pattern posea **capacidades** (habilidades) para realizar acciones complejas de forma type-safe, testeable y mantenible.

### ¿Por qué Abilities?

**Antes (sin Abilities):**
```csharp
// ❌ Magic strings, casting manual, acoplamiento
_scenarioContext["user"] = await _dbConnection.QueryFirstAsync<User>(...);
var user = (User)_scenarioContext["user"];
```

**Después (con Abilities):**
```csharp
// ✅ Type-safe, expresivo, desacoplado
var user = await _actor.Using<AccessDatabase>().GetUserByIdAsync(1);
_actor.Using<RememberData>().Remember("user", user);
var storedUser = _actor.Using<RememberData>().Recall<User>("user");
```

### Ventajas
- ✅ **Type Safety**: Sin castings manuales ni magic strings
- ✅ **Screenplay Puro**: Mantiene la filosofía del patrón
- ✅ **Testeable**: Abilities mockables para unit tests
- ✅ **Fluent API**: Código expresivo (`actor.WhoCan(...).Using<...>()`)
- ✅ **Desacoplamiento**: Steps no conocen infraestructura (DB, API)
- ✅ **Lifecycle**: Cleanup automático de recursos

---

## Conceptos Fundamentales

### IAbility Interface

Todas las abilities implementan `IAbility`:

```csharp
public interface IAbility
{
    /// <summary>
    /// Inicializa la ability (opcional).
    /// Ejecutado automáticamente al asignar con WhoCan().
    /// </summary>
    Task InitializeAsync() => Task.CompletedTask;
    
    /// <summary>
    /// Limpia recursos de la ability (opcional).
    /// Ejecutado en AfterScenario con actor.CleanupAsync().
    /// </summary>
    Task CleanupAsync() => Task.CompletedTask;
}
```

### Actor con Abilities

El **Actor** es el protagonista que posee abilities:

```csharp
public class Actor
{
    public string Name { get; }
    public IPage Page { get; }
    
    // Asignar una ability (fluent)
    public Actor WhoCan(IAbility ability);
    
    // Usar una ability (type-safe)
    public T Using<T>() where T : IAbility;
    
    // Verificar si tiene una ability
    public bool Has<T>() where T : IAbility;
    
    // Intentar usar una ability (safe)
    public bool TryUsing<T>(out T? ability) where T : IAbility;
    
    // Limpiar todas las abilities
    public Task CleanupAsync();
    
    // Ejecutar una Task (Screenplay)
    public Task AttemptsTo(ITask task);
}
```

### Flujo de Uso

```
1. BeforeScenario
   ↓
2. Asignar Abilities con WhoCan()
   actor.WhoCan(new RememberData())
        .WhoCan(new AccessDatabase(config));
   ↓
3. Step Bindings
   Usar abilities con Using<T>()
   var user = actor.Using<AccessDatabase>().GetUserByIdAsync(1);
   actor.Using<RememberData>().Remember("user", user);
   ↓
4. AfterScenario
   Cleanup con actor.CleanupAsync();
```

---

## Abilities Disponibles

### 1. RememberData

**Propósito**: Almacenamiento type-safe en memoria para compartir datos entre steps.

**Cuándo usar**:
- Compartir datos entre steps de un mismo escenario
- Almacenar resultados de queries/API calls para verificación posterior
- Evitar magic strings y castings en ScenarioContext

**API**:
```csharp
public class RememberData : IAbility
{
    // Almacenar valor con type safety
    void Remember<T>(string key, T value);
    
    // Recuperar valor (lanza InvalidCastException si tipo no coincide)
    T? Recall<T>(string key);
    
    // Verificar existencia de clave
    bool Has(string key);
    
    // Olvidar una clave
    bool Forget(string key);
    
    // Olvidar todas las claves
    void ForgetAll();
    
    // Listar claves almacenadas
    IReadOnlyCollection<string> Keys { get; }
}
```

**Ejemplo**:
```csharp
// Almacenar
var user = new User { Username = "john", Email = "john@example.com" };
_actor.Using<RememberData>().Remember("currentUser", user);

// Recuperar (type-safe)
var storedUser = _actor.Using<RememberData>().Recall<User>("currentUser");

// Verificar
if (_actor.Using<RememberData>().Has("currentUser"))
{
    // ...
}

// Olvidar
_actor.Using<RememberData>().Forget("currentUser");
```

---

### 2. AccessDatabase

**Propósito**: Ejecutar queries y comandos SQL con retry logic y metadata tracking.

**Cuándo usar**:
- Obtener datos de prueba dinámicos de la DB
- Crear/limpiar datos de test setup
- Validar que operaciones UI guardaron correctamente en DB

**Dependencias**:
- Dapper 2.1.35
- IDbConnection (inyectado en constructor)
- DatabaseConfig (retry settings, connection string)

**API**:
```csharp
public class AccessDatabase : IAbility
{
    // Queries genéricas
    Task<QueryResult<T>> QueryAsync<T>(string sql, object? parameters = null);
    Task<T?> QuerySingleAsync<T>(string sql, object? parameters = null);
    Task<int> ExecuteAsync(string sql, object? parameters = null);
    
    // Helpers específicos (ejemplo: Users)
    Task<List<User>> GetActiveUsersAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<int> CreateUserAsync(User user);
    Task<bool> DeleteUserAsync(int userId);
}
```

**QueryResult<T>** (metadata):
```csharp
public class QueryResult<T>
{
    public List<T> Data { get; set; }        // Resultados
    public int RowCount { get; set; }        // Cantidad de filas
    public TimeSpan ExecutionTime { get; set; } // Duración
    public DateTime Timestamp { get; set; }   // Timestamp
}
```

**Ejemplo**:
```csharp
// Setup en BeforeScenario
var dbConfig = new DatabaseConfig
{
    ConnectionString = "Server=localhost;Database=TestDB;Trusted_Connection=true;",
    Provider = "SqlServer",
    RetryCount = 3,
    RetryDelayMs = 500
};

var dbConnection = new SqlConnection(dbConfig.ConnectionString);
_actor = _actor.WhoCan(new AccessDatabase(dbConnection, dbConfig));

// Usar en Steps
var users = await _actor.Using<AccessDatabase>().GetActiveUsersAsync();
var user = users.First();

var specificUser = await _actor.Using<AccessDatabase>()
    .GetUserByUsernameAsync("john.doe");

// Query personalizada
var result = await _actor.Using<AccessDatabase>().QueryAsync<User>(
    "SELECT * FROM Users WHERE Role = @Role AND Active = 1",
    new { Role = "Admin" }
);

Console.WriteLine($"Found {result.RowCount} admins in {result.ExecutionTime.TotalMilliseconds}ms");
```

**Retry Logic**:
AccessDatabase implementa retry automático con exponential backoff:
- Reintentos configurables (default: 3)
- Delay entre reintentos (default: 500ms)
- Útil para flakiness en entornos CI/CD

---

### 3. CallApiEndpoint

**Propósito**: Realizar llamadas HTTP/API sin necesidad de UI.

**Cuándo usar**:
- Escenarios híbridos (crear datos vía API, validar en UI)
- Test de integración UI + Backend
- Setup/Teardown de datos de prueba

**Dependencias**:
- IAPIRequestContext (Playwright)
- AppSettings (para base URLs)

**API**:
```csharp
public class CallApiEndpoint : IAbility
{
    // GET request
    Task<T?> GetAsync<T>(string endpoint, string? baseUrl = null);
    
    // POST request
    Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint, TRequest body, string? baseUrl = null);
    
    // Request genérico (PUT, DELETE, PATCH, etc.)
    Task<IAPIResponse> RequestAsync(
        string method, string endpoint, APIRequestContextOptions? options = null);
}
```

**Ejemplo**:
```csharp
// Setup
var apiContext = await playwright.APIRequest.NewContextAsync();
_actor = _actor.WhoCan(new CallApiEndpoint(apiContext, _settings));

// GET
var booking = await _actor.Using<CallApiEndpoint>()
    .GetAsync<Booking>("/booking/123");

// POST
var newBooking = new BookingRequest
{
    FirstName = "John",
    LastName = "Doe",
    CheckIn = "2024-01-15",
    CheckOut = "2024-01-20"
};

var response = await _actor.Using<CallApiEndpoint>()
    .PostAsync<BookingRequest, BookingResponse>(
        "/booking", 
        newBooking,
        "https://restful-booker.herokuapp.com"
    );

_actor.Using<RememberData>().Remember("bookingId", response.BookingId);

// Request genérico (DELETE)
var deleteResponse = await _actor.Using<CallApiEndpoint>().RequestAsync(
    "DELETE",
    $"/booking/{response.BookingId}",
    new() { Headers = new Dictionary<string, string> { ["Authorization"] = "Basic abc123" } }
);
```

**Escenario Híbrido Completo**:
```gherkin
@api-hybrid
Scenario: Crear booking vía API y validar en UI
  Given creo un booking vía API
  When navego a la página de bookings
  Then debo ver mi booking en la lista
```

```csharp
[Given(@"creo un booking vía API")]
public async Task CrearBookingViaApi()
{
    var booking = new BookingRequest { /* ... */ };
    
    var response = await _actor.Using<CallApiEndpoint>()
        .PostAsync<BookingRequest, BookingResponse>("/booking", booking);
    
    _actor.Using<RememberData>().Remember("apiBooking", response);
}

[Then(@"debo ver mi booking en la lista")]
public void ValidarBookingEnUi()
{
    var booking = _actor.Using<RememberData>().Recall<BookingResponse>("apiBooking");
    
    _actor.AttemptsTo(
        Navigate.To(_settings.BaseUrl + "/bookings"),
        Verify.That(BookingsPage.GetBookingRow(booking.BookingId), Is.Visible)
    );
}
```

---

### 4. ReadConfiguration

**Propósito**: Acceso type-safe a AppSettings sin acoplar steps a ConfigManager.

**Cuándo usar**:
- Leer URLs base dinámicamente según ambiente
- Obtener credenciales de test users
- Acceder a feature flags o configuración del framework

**Dependencias**:
- AppSettings (inyectado en constructor)

**API**:
```csharp
public class ReadConfiguration : IAbility
{
    // Settings básicos
    string GetBaseUrl();
    bool IsHeadless();
    string GetBrowser();
    string GetEnvironment();
    AppSettings GetSettings(); // Full access
    
    // Credenciales
    (string Username, string Password) GetCredentials(string service);
    
    // API URLs
    string GetApiUrl(string apiName);
}
```

**Ejemplo**:
```csharp
// Setup
var settings = ConfigManager.Settings;
_actor = _actor.WhoCan(new ReadConfiguration(settings));

// Uso en steps
var baseUrl = _actor.Using<ReadConfiguration>().GetBaseUrl();
await _actor.AttemptsTo(Navigate.To(baseUrl));

var (username, password) = _actor.Using<ReadConfiguration>()
    .GetCredentials("admin");
await _actor.AttemptsTo(Login.WithCredentials(username, password));

var apiUrl = _actor.Using<ReadConfiguration>().GetApiUrl("restfulbooker");

if (_actor.Using<ReadConfiguration>().IsHeadless())
{
    // Skip visual tests in headless
}
```

---

## Guía de Uso

### Setup en BeforeScenario

```csharp
[Binding]
public class Hooks
{
    private Actor? _actor;
    
    [BeforeScenario]
    public async Task BeforeScenario()
    {
        // 1. Obtener dependencies
        var page = await GetPlaywrightPageAsync();
        var apiContext = await GetApiContextAsync();
        var dbConnection = GetDatabaseConnection();
        var settings = ConfigManager.Settings;
        
        var dbConfig = new DatabaseConfig
        {
            ConnectionString = settings.ConnectionString,
            Provider = "SqlServer",
            RetryCount = 3,
            RetryDelayMs = 500
        };
        
        // 2. Crear Actor con Abilities (fluent API)
        _actor = new Actor("TestUser", page)
            .WhoCan(new RememberData())
            .WhoCan(new AccessDatabase(dbConnection, dbConfig))
            .WhoCan(new CallApiEndpoint(apiContext, settings))
            .WhoCan(new ReadConfiguration(settings));
    }
    
    [AfterScenario]
    public async Task AfterScenario()
    {
        // 3. Cleanup (cierra conexiones, limpia memoria)
        if (_actor != null)
            await _actor.CleanupAsync();
    }
}
```

### Uso en Step Bindings

```csharp
[Binding]
public class LoginStepBindings
{
    private readonly Actor _actor;
    
    public LoginStepBindings(Actor actor)
    {
        _actor = actor;
    }
    
    [When(@"inicio sesión con usuario de base de datos")]
    public async Task LoginConUsuarioDb()
    {
        // 1. Obtener datos de DB
        var users = await _actor.Using<AccessDatabase>()
            .GetActiveUsersAsync();
        var user = users.First();
        
        // 2. Guardar para uso posterior
        _actor.Using<RememberData>().Remember("currentUser", user);
        
        // 3. Usar en Task de Screenplay
        await _actor.AttemptsTo(
            Login.WithCredentials(user.Username, user.Password)
        );
    }
    
    [Then(@"verifico que el nombre mostrado es correcto")]
    public void VerificarNombre()
    {
        // Type-safe recall
        var user = _actor.Using<RememberData>().Recall<User>("currentUser");
        
        _actor.AttemptsTo(
            Verify.That(UserPage.DisplayName, Is.EqualTo(user.FullName))
        );
    }
}
```

### Verificación de Abilities

```csharp
// Verificar si Actor tiene una ability
if (_actor.Has<AccessDatabase>())
{
    var users = await _actor.Using<AccessDatabase>().GetActiveUsersAsync();
}

// Safe retrieval (no lanza excepción)
if (_actor.TryUsing<RememberData>(out var rememberData))
{
    var user = rememberData.Recall<User>("currentUser");
}
else
{
    // Actor no tiene RememberData
}
```

---

## Ejemplos Prácticos

### Ejemplo 1: Login con Usuario de DB

**Feature**:
```gherkin
@database
Scenario: Login con usuario activo de la base de datos
  Given tengo un usuario activo en la base de datos
  When inicio sesión con las credenciales del usuario
  Then debo ver la página de inicio correctamente
```

**Step Bindings**:
```csharp
[Given(@"tengo un usuario activo en la base de datos")]
public async Task TengoUsuarioActivoDb()
{
    var users = await _actor.Using<AccessDatabase>()
        .QueryAsync<User>("SELECT TOP 1 * FROM Users WHERE Active = 1");
    
    Assert.That(users.RowCount, Is.GreaterThan(0), "Debe haber al menos 1 usuario activo");
    
    _actor.Using<RememberData>().Remember("selectedUser", users.Data.First());
}

[When(@"inicio sesión con las credenciales del usuario")]
public async Task InicioSesionConCredenciales()
{
    var user = _actor.Using<RememberData>().Recall<User>("selectedUser");
    
    await _actor.AttemptsTo(
        Navigate.To(_actor.Using<ReadConfiguration>().GetBaseUrl()),
        Login.WithCredentials(user.Username, user.Password)
    );
}

[Then(@"debo ver la página de inicio correctamente")]
public void VeoHomeCorrectamente()
{
    var user = _actor.Using<RememberData>().Recall<User>("selectedUser");
    
    _actor.AttemptsTo(
        Verify.That(HomePage.WelcomeMessage, Contains.Substring(user.FirstName)),
        Verify.That(HomePage.UserMenu, Is.Visible)
    );
}
```

---

### Ejemplo 2: Escenario Híbrido (API + UI)

**Feature**:
```gherkin
@api-hybrid
Scenario: Crear booking vía API y validar en UI
  Given creo un booking para "John Doe" vía API
  When navego a la página de bookings
  And busco el booking creado
  Then debo ver los detalles del booking correctamente
```

**Step Bindings**:
```csharp
[Given(@"creo un booking para ""(.*)"" vía API")]
public async Task CreoBookingViaApi(string guestName)
{
    var parts = guestName.Split(' ');
    var booking = new BookingRequest
    {
        FirstName = parts[0],
        LastName = parts[1],
        TotalPrice = 250,
        DepositPaid = true,
        CheckIn = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd"),
        CheckOut = DateTime.Today.AddDays(10).ToString("yyyy-MM-dd"),
        AdditionalNeeds = "Breakfast"
    };
    
    var response = await _actor.Using<CallApiEndpoint>()
        .PostAsync<BookingRequest, BookingResponse>(
            "/booking",
            booking,
            _actor.Using<ReadConfiguration>().GetApiUrl("restfulbooker")
        );
    
    Assert.That(response, Is.Not.Null);
    Assert.That(response.BookingId, Is.GreaterThan(0));
    
    _actor.Using<RememberData>().Remember("apiBooking", response);
}

[When(@"navego a la página de bookings")]
public async Task NavegoBookings()
{
    var baseUrl = _actor.Using<ReadConfiguration>().GetBaseUrl();
    await _actor.AttemptsTo(Navigate.To($"{baseUrl}/bookings"));
}

[When(@"busco el booking creado")]
public async Task BuscoBookingCreado()
{
    var booking = _actor.Using<RememberData>().Recall<BookingResponse>("apiBooking");
    
    await _actor.AttemptsTo(
        Fill.In(BookingsPage.SearchInput, booking.BookingId.ToString()),
        Click.On(BookingsPage.SearchButton)
    );
}

[Then(@"debo ver los detalles del booking correctamente")]
public void VeoDetallesBooking()
{
    var booking = _actor.Using<RememberData>().Recall<BookingResponse>("apiBooking");
    
    _actor.AttemptsTo(
        Verify.That(BookingsPage.BookingGuestName, Is.EqualTo($"{booking.Booking.FirstName} {booking.Booking.LastName}")),
        Verify.That(BookingsPage.BookingPrice, Is.EqualTo(booking.Booking.TotalPrice.ToString())),
        Verify.That(BookingsPage.BookingStatus, Contains.Substring("Confirmed"))
    );
}
```

---

### Ejemplo 3: Data-Driven con DB

**Feature**:
```gherkin
@database @data-driven
Scenario: Validar login con múltiples usuarios de la DB
  Given tengo 3 usuarios activos en la base de datos
  When inicio sesión con cada usuario
  Then todos deben acceder exitosamente
```

**Step Bindings**:
```csharp
[Given(@"tengo (.*) usuarios activos en la base de datos")]
public async Task TengoUsuariosActivos(int count)
{
    var result = await _actor.Using<AccessDatabase>().QueryAsync<User>(
        "SELECT TOP (@Count) * FROM Users WHERE Active = 1 ORDER BY NEWID()",
        new { Count = count }
    );
    
    Assert.That(result.RowCount, Is.EqualTo(count));
    
    _actor.Using<RememberData>().Remember("testUsers", result.Data);
}

[When(@"inicio sesión con cada usuario")]
public async Task InicioSesionConCadaUsuario()
{
    var users = _actor.Using<RememberData>().Recall<List<User>>("testUsers");
    var results = new List<bool>();
    
    foreach (var user in users)
    {
        await _actor.AttemptsTo(
            Navigate.To(_actor.Using<ReadConfiguration>().GetBaseUrl()),
            Login.WithCredentials(user.Username, user.Password)
        );
        
        var loginSuccess = await _actor.Page.Locator(HomePage.WelcomeMessage).IsVisibleAsync();
        results.Add(loginSuccess);
        
        // Logout para siguiente usuario
        await _actor.AttemptsTo(Click.On(HomePage.LogoutButton));
    }
    
    _actor.Using<RememberData>().Remember("loginResults", results);
}

[Then(@"todos deben acceder exitosamente")]
public void TodosAccedenExitosamente()
{
    var results = _actor.Using<RememberData>().Recall<List<bool>>("loginResults");
    
    Assert.That(results, Is.All.True, "Todos los usuarios deben loguearse exitosamente");
}
```

---

## Crear Custom Abilities

### Paso 1: Implementar IAbility

```csharp
using QuantumTestSuite.UI.Screenplay.Abilities;

public class SendEmail : IAbility
{
    private readonly SmtpClient _smtpClient;
    private readonly EmailConfig _config;
    
    public SendEmail(EmailConfig config)
    {
        _config = config;
        _smtpClient = new SmtpClient(_config.SmtpHost, _config.SmtpPort)
        {
            Credentials = new NetworkCredential(_config.Username, _config.Password),
            EnableSsl = _config.UseSsl
        };
    }
    
    public async Task SendAsync(string to, string subject, string body)
    {
        var message = new MailMessage(_config.FromAddress, to, subject, body);
        await _smtpClient.SendMailAsync(message);
    }
    
    public async Task InitializeAsync()
    {
        // Opcional: validar conexión SMTP
        Console.WriteLine($"SendEmail ability initialized for {_config.SmtpHost}");
        return Task.CompletedTask;
    }
    
    public async Task CleanupAsync()
    {
        // Cerrar conexión SMTP
        _smtpClient?.Dispose();
        return Task.CompletedTask;
    }
}
```

### Paso 2: Asignar al Actor

```csharp
var emailConfig = new EmailConfig
{
    SmtpHost = "smtp.gmail.com",
    SmtpPort = 587,
    Username = "test@example.com",
    Password = "app-password",
    FromAddress = "test@example.com",
    UseSsl = true
};

_actor = _actor.WhoCan(new SendEmail(emailConfig));
```

### Paso 3: Usar en Steps

```csharp
[When(@"envío un email de verificación a ""(.*)""")]
public async Task EnvioEmailVerificacion(string email)
{
    await _actor.Using<SendEmail>().SendAsync(
        to: email,
        subject: "Verify Your Account",
        body: "Click the link to verify..."
    );
}
```

---

## Best Practices

### 1. Inicializar Abilities en BeforeScenario

```csharp
// ✅ Correcto: en BeforeScenario
[BeforeScenario]
public void Setup()
{
    _actor = new Actor("TestUser", page)
        .WhoCan(new RememberData())
        .WhoCan(new ReadConfiguration(settings));
}

// ❌ Incorrecto: en cada step
[When(@"...")]
public void Step()
{
    _actor.WhoCan(new RememberData()); // Re-asigna innecesariamente
}
```

### 2. Siempre Cleanup en AfterScenario

```csharp
[AfterScenario]
public async Task Cleanup()
{
    if (_actor != null)
        await _actor.CleanupAsync(); // ⚠️ IMPORTANTE
}
```

### 3. Usar Type-Safe Keys en RememberData

```csharp
// ✅ Constantes para keys
public static class DataKeys
{
    public const string CurrentUser = "currentUser";
    public const string BookingId = "bookingId";
}

_actor.Using<RememberData>().Remember(DataKeys.CurrentUser, user);
var user = _actor.Using<RememberData>().Recall<User>(DataKeys.CurrentUser);
```

### 4. Verificar Abilities Antes de Usar (Optional)

```csharp
if (!_actor.Has<AccessDatabase>())
{
    throw new InvalidOperationException(
        "This scenario requires AccessDatabase ability. Add @database tag.");
}
```

### 5. No Abusar de RememberData

```csharp
// ❌ Mal: pasar todo por RememberData
_actor.Using<RememberData>().Remember("username", user.Username);
_actor.Using<RememberData>().Remember("password", user.Password);
_actor.Using<RememberData>().Remember("email", user.Email);

// ✅ Mejor: almacenar el objeto completo
_actor.Using<RememberData>().Remember("currentUser", user);
```

### 6. Retry Logic en AccessDatabase

```csharp
// Ya implementado en AccessDatabase
var users = await _actor.Using<AccessDatabase>().GetActiveUsersAsync();
// Si falla, reintenta automáticamente (3x con 500ms delay)
```

### 7. Separar Setup Data de Business Logic

```csharp
// ✅ Correcto: Given setup, When business logic
[Given(@"tengo un usuario en DB")]
public async Task SetupUsuario()
{
    var user = await _actor.Using<AccessDatabase>().CreateUserAsync(newUser);
    _actor.Using<RememberData>().Remember("user", user);
}

[When(@"inicio sesión")]
public async Task Login()
{
    var user = _actor.Using<RememberData>().Recall<User>("user");
    await _actor.AttemptsTo(Login.WithCredentials(user.Username, user.Password));
}
```

---

## Troubleshooting

### Error: AbilityNotFoundException

**Mensaje**:
```
Actor 'TestUser' does not have the ability 'AccessDatabase'.
Did you forget to call: actor.WhoCan(new AccessDatabase(...))?
```

**Solución**:
```csharp
// Asegúrate de asignar la ability en BeforeScenario
_actor = _actor.WhoCan(new AccessDatabase(connection, config));
```

---

### Error: InvalidCastException en RememberData

**Mensaje**:
```
Unable to cast object of type 'User' to type 'System.String'.
Data 'currentUser' is QuantumTestSuite.Core.Models.User, not System.String.
```

**Causa**:
```csharp
_actor.Using<RememberData>().Remember("currentUser", user);
var username = _actor.Using<RememberData>().Recall<string>("currentUser"); // ❌ Tipo incorrecto
```

**Solución**:
```csharp
var user = _actor.Using<RememberData>().Recall<User>("currentUser"); // ✅ Tipo correcto
var username = user.Username;
```

---

### Error: Connection String Not Found (AccessDatabase)

**Solución**:
```csharp
// Verificar configuración en appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TestDB;..."
  }
}

// O en .env
CONNECTION_STRING=Server=localhost;Database=TestDB;...
```

---

### Abilities No Se Limpian Correctamente

**Problema**: Datos persisten entre escenarios.

**Solución**:
```csharp
[AfterScenario]
public async Task Cleanup()
{
    await _actor?.CleanupAsync(); // ⚠️ No olvidar el '?.' por si actor es null
}
```

---

### Performance: Muchas Queries Lentas

**Solución**:
```csharp
// Opción 1: Aumentar retry delay
var config = new DatabaseConfig
{
    RetryCount = 5,
    RetryDelayMs = 1000 // Aumentar si DB está bajo carga
};

// Opción 2: Cachear usuarios en RememberData (setup)
[BeforeScenario]
public async Task CachearUsuarios()
{
    var users = await _actor.Using<AccessDatabase>().GetActiveUsersAsync();
    _actor.Using<RememberData>().Remember("cachedUsers", users);
}

// Usar cache en steps
var users = _actor.Using<RememberData>().Recall<List<User>>("cachedUsers");
```

---

## Referencias

- [ADR-005: Abilities Pattern](../ADRs/005-abilities-pattern.md)
- [Screenplay Pattern - Serenity BDD](https://serenity-bdd.github.io/docs/screenplay/screenplay_fundamentals)
- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [Playwright API Request Context](https://playwright.dev/dotnet/docs/api/class-apirequestcontext)

---

**Última Actualización**: 2024-01-15  
**Mantenido por**: QuantumTestSuite Team
