# ADR 005: Abilities Pattern para Screenplay

## Estado
Aceptado

## Fecha
2024-01-15

## Contexto

### Problema
Al implementar el Screenplay Pattern en QuantumTestSuite, identificamos limitaciones para manejar escenarios complejos que requieren:

1. **Consultas a bases de datos** para obtener datos de prueba dinámicos
2. **Llamadas API mixtas** (UI + API en el mismo escenario)
3. **Estado compartido entre steps** de forma type-safe
4. **Acceso a configuración** sin acoplar steps a ConfigManager

Los approaches previos tenían problemas:
- **DataTable/Dictionary<string, object>**: Magic strings, sin type safety, propenso a errores
- **ScenarioContext directo**: Acoplamiento, difícil de testear, no sigue Screenplay
- **Dependencias directas**: Steps acoplados a infraestructura (DB, API clients)

### Ejemplo del problema
```csharp
[When(@"inicio sesión con un usuario activo de la base de datos")]
public async Task LoginWithDbUser()
{
    // ❌ Acoplamiento directo a DB
    var user = await _dbConnection.QueryFirstAsync<User>("SELECT * FROM Users WHERE Active = 1");
    
    // ❌ Magic strings para compartir estado
    _scenarioContext["currentUser"] = user;
    
    await _actor.AttemptsTo(Login.WithCredentials(user.Username, user.Password));
}

[Then(@"verifico que el nombre mostrado coincide con el usuario")]
public void VerifyUserName()
{
    // ❌ Casting manual, propenso a errores
    var user = (User)_scenarioContext["currentUser"];
    
    _actor.AttemptsTo(Verify.That(UserPage.DisplayName, Is.EqualTo(user.FullName)));
}
```

## Decisión

Implementamos el **Abilities Pattern** del Screenplay Pattern puro, donde cada **Actor** posee **Abilities** (habilidades) que le permiten realizar **Tasks** a través de **Interactions**.

### Principios del Patrón

1. **Abilities como Capacidades**: Una Ability representa una capacidad que el Actor posee
   - `RememberData`: Memoria type-safe para compartir datos entre steps
   - `AccessDatabase`: Ejecutar queries y comandos SQL
   - `CallApiEndpoint`: Realizar llamadas HTTP/API
   - `ReadConfiguration`: Acceder a configuración del framework

2. **Fluent API**: Asignación y uso expresivo
   ```csharp
   actor.WhoCan(new RememberData())
        .WhoCan(new AccessDatabase(dbConfig));
   
   var user = actor.Using<RememberData>().Recall<User>("currentUser");
   ```

3. **Type Safety**: Sin magic strings ni castings manuales
   ```csharp
   // ✅ Type-safe
   actor.Using<RememberData>().Remember("user", myUser);
   var user = actor.Using<RememberData>().Recall<User>("user");
   
   // ❌ Excepción en tiempo de ejecución si no coincide el tipo
   var wrongType = actor.Using<RememberData>().Recall<string>("user"); // InvalidCastException
   ```

4. **Lifecycle Management**: Inicialización y limpieza automática
   ```csharp
   public interface IAbility
   {
       Task InitializeAsync() => Task.CompletedTask;
       Task CleanupAsync() => Task.CompletedTask;
   }
   ```

### Arquitectura

```
┌─────────────────────────────────────────────────┐
│              Actor (TestUser)                   │
│  Name: "TestUser"                               │
│  Page: IPage                                     │
├─────────────────────────────────────────────────┤
│              Abilities                          │
│  • RememberData                                 │
│  • AccessDatabase                               │
│  • CallApiEndpoint                              │
│  • ReadConfiguration                            │
└─────────────────────────────────────────────────┘
        │
        │ AttemptsTo(Task)
        ▼
┌─────────────────────────────────────────────────┐
│              Task (Login)                       │
│  PerformAs(actor):                              │
│    1. var user = actor.Using<RememberData>()    │
│                     .Recall<User>("user")       │
│    2. actor.AttemptsTo(                         │
│         Fill.In(UsernameField, user.Username))  │
│    3. actor.AttemptsTo(                         │
│         Click.On(LoginButton))                  │
└─────────────────────────────────────────────────┘
        │
        │ PerformAs(actor)
        ▼
┌─────────────────────────────────────────────────┐
│          Interaction (Fill, Click)              │
│  Acciones directas sobre la UI                  │
└─────────────────────────────────────────────────┘
```

### Implementación

#### 1. Interface Base
```csharp
public interface IAbility
{
    Task InitializeAsync() => Task.CompletedTask;
    Task CleanupAsync() => Task.CompletedTask;
}
```

#### 2. Actor Extendido
```csharp
public class Actor
{
    private readonly Dictionary<Type, IAbility> _abilities = new();
    
    public Actor WhoCan(IAbility ability)
    {
        _abilities[ability.GetType()] = ability;
        return this;
    }
    
    public T Using<T>() where T : IAbility
    {
        if (_abilities.TryGetValue(typeof(T), out var ability))
            return (T)ability;
        
        throw new AbilityNotFoundException(Name, typeof(T));
    }
    
    public bool Has<T>() where T : IAbility => _abilities.ContainsKey(typeof(T));
    
    public async Task CleanupAsync()
    {
        foreach (var ability in _abilities.Values)
            await ability.CleanupAsync();
    }
}
```

#### 3. Abilities Concretas

**RememberData** (In-Memory Storage)
```csharp
public class RememberData : IAbility
{
    private readonly Dictionary<string, object> _data = new();
    
    public void Remember<T>(string key, T value) => _data[key] = value!;
    
    public T? Recall<T>(string key)
    {
        if (!_data.TryGetValue(key, out var value))
            return default;
        
        if (value is T typedValue)
            return typedValue;
        
        throw new InvalidCastException($"Data '{key}' is {value.GetType()}, not {typeof(T)}");
    }
    
    public bool Has(string key) => _data.ContainsKey(key);
    public bool Forget(string key) => _data.Remove(key);
    public void ForgetAll() => _data.Clear();
    
    public IReadOnlyCollection<string> Keys => _data.Keys.ToList();
    
    public Task CleanupAsync()
    {
        ForgetAll();
        return Task.CompletedTask;
    }
}
```

**AccessDatabase** (Dapper + Retry Logic)
```csharp
public class AccessDatabase : IAbility
{
    private readonly IDbConnection _connection;
    private readonly DatabaseConfig _config;
    
    public async Task<QueryResult<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        var startTime = DateTime.UtcNow;
        
        var results = await RetryAsync(async () =>
        {
            await EnsureConnectionOpenAsync();
            return (await _connection.QueryAsync<T>(sql, parameters)).ToList();
        });
        
        var duration = DateTime.UtcNow - startTime;
        
        return new QueryResult<T>
        {
            Data = results,
            RowCount = results.Count,
            ExecutionTime = duration,
            Timestamp = startTime
        };
    }
    
    public async Task<List<User>> GetActiveUsersAsync()
    {
        var result = await QueryAsync<User>(
            "SELECT * FROM Users WHERE Active = 1 ORDER BY CreatedAt DESC"
        );
        return result.Data;
    }
    
    // ... más métodos helpers (GetUserById, CreateUser, etc.)
}
```

**CallApiEndpoint** (API Requests)
```csharp
public class CallApiEndpoint : IAbility
{
    private readonly IAPIRequestContext _apiContext;
    private readonly AppSettings _settings;
    
    public async Task<T?> GetAsync<T>(string endpoint, string? baseUrl = null)
    {
        var url = $"{baseUrl ?? _settings.ApiBaseUrl}{endpoint}";
        var response = await _apiContext.GetAsync(url);
        
        if (!response.Ok)
            throw new HttpRequestException($"GET {url} failed: {response.Status}");
        
        return await response.JsonAsync<T>();
    }
    
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint, TRequest body, string? baseUrl = null)
    {
        var url = $"{baseUrl ?? _settings.ApiBaseUrl}{endpoint}";
        var response = await _apiContext.PostAsync(url, new() { DataObject = body });
        
        if (!response.Ok)
            throw new HttpRequestException($"POST {url} failed: {response.Status}");
        
        return await response.JsonAsync<TResponse>();
    }
}
```

**ReadConfiguration** (AppSettings Access)
```csharp
public class ReadConfiguration : IAbility
{
    private readonly AppSettings _settings;
    
    public ReadConfiguration(AppSettings settings) => _settings = settings;
    
    public string GetBaseUrl() => _settings.BaseUrl;
    public bool IsHeadless() => _settings.Headless;
    public string GetBrowser() => _settings.Browser;
    public string GetEnvironment() => _settings.Environment;
    
    public (string Username, string Password) GetCredentials(string service) =>
        service.ToLower() switch
        {
            "admin" => (_settings.AdminUsername, _settings.AdminPassword),
            "user" => (_settings.UserUsername, _settings.UserPassword),
            _ => throw new ArgumentException($"Unknown service: {service}")
        };
    
    public string GetApiUrl(string apiName) =>
        apiName.ToLower() switch
        {
            "restfulbooker" => _settings.RestfulBookerApiUrl,
            "github" => _settings.GitHubApiUrl,
            _ => _settings.ApiBaseUrl
        };
}
```

### Uso en Step Bindings

```csharp
[Binding]
public class LoginStepBindings
{
    private readonly Actor _actor;
    
    [BeforeScenario]
    public async Task Setup()
    {
        var page = await _playwright.Chromium.LaunchAsync();
        var dbConfig = ConfigManager.GetDatabaseConfig();
        var settings = ConfigManager.Settings;
        
        _actor = new Actor("TestUser", page)
            .WhoCan(new RememberData())
            .WhoCan(new AccessDatabase(dbConfig))
            .WhoCan(new CallApiEndpoint(apiContext, settings))
            .WhoCan(new ReadConfiguration(settings));
    }
    
    [When(@"inicio sesión con un usuario activo de la base de datos")]
    public async Task LoginWithDbUser()
    {
        // ✅ Screenplay Pattern puro
        var users = await _actor.Using<AccessDatabase>().GetActiveUsersAsync();
        var user = users.First();
        
        _actor.Using<RememberData>().Remember("currentUser", user);
        
        await _actor.AttemptsTo(Login.WithCredentials(user.Username, user.Password));
    }
    
    [Then(@"verifico que el nombre mostrado coincide con el usuario")]
    public void VerifyUserName()
    {
        // ✅ Type-safe, sin casting
        var user = _actor.Using<RememberData>().Recall<User>("currentUser");
        
        _actor.AttemptsTo(
            Verify.That(UserPage.DisplayName, Is.EqualTo(user.FullName))
        );
    }
    
    [AfterScenario]
    public async Task Cleanup()
    {
        await _actor.CleanupAsync(); // Limpia todas las abilities
    }
}
```

## Consecuencias

### Positivas ✅

1. **Screenplay Puro**: Mantiene la filosofía original del patrón
2. **Type Safety**: Eliminación de magic strings y castings manuales
3. **Testabilidad**: Abilities fácilmente mockables para unit tests
4. **Separation of Concerns**: Steps no conocen infraestructura (DB, HTTP)
5. **Fluent API**: Código expresivo y legible (`actor.WhoCan(...).Using<...>()`)
6. **Extensibilidad**: Nuevas abilities sin modificar Actor
7. **Lifecycle**: Cleanup automático de recursos (conexiones, memoria)

### Negativas ⚠️

1. **Curva de Aprendizaje**: Equipo debe entender Screenplay + Abilities
2. **Más Clases**: Mayor cantidad de archivos (cada Ability es una clase)
3. **Overhead Inicial**: Setup de abilities en BeforeScenario
4. **Abstracción Extra**: Una capa más entre step y acción real

### Neutras ℹ️

1. **Migración**: Código existente puede convivir, migración gradual
2. **Complejidad**: Apropiada para proyectos enterprise, posible over-engineering para proyectos simples

## Alternativas Consideradas

### Alternative 1: Context Extensions
```csharp
_context.RememberUser(user);
var user = _context.RecallUser();
```
**Rechazada**: Rompe Screenplay Pattern, acoplamiento a ScenarioContext

### Alternative 2: Static/Singleton Blackboard
```csharp
Blackboard.Set("user", user);
var user = Blackboard.Get<User>("user");
```
**Rechazada**: Estado global difícil de testear, no thread-safe

### Alternative 3: Domain-Specific Contexts
```csharp
_loginContext.CurrentUser = user;
await _bookingContext.CreateBooking(booking);
```
**Rechazada**: Proliferación de contextos, no sigue Screenplay

## Referencias

- [Screenplay Pattern - Serenity BDD](https://serenity-bdd.github.io/docs/screenplay/screenplay_fundamentals)
- [Abilities in Screenplay - Antony Marcano](https://ideas.riverglide.com/page-objects-refactored-12ec3541990#.xqkw8qmwh)
- [Lean Page Objects - Serenity JS](https://serenity-js.org/handbook/design/screenplay-pattern/)

## Notas de Implementación

### Dependencies
- **Dapper 2.1.35**: Para AccessDatabase ability
- **System.Data.Common**: Para IDbConnection
- **Microsoft.Playwright**: Para CallApiEndpoint (IAPIRequestContext)

### Consideraciones de Performance
- **RememberData**: Dictionary in-memory, limpieza en AfterScenario
- **AccessDatabase**: Connection pooling, retry logic con Polly
- **CallApiEndpoint**: Reutiliza IAPIRequestContext de Playwright

### Patterns Aplicados
- **Strategy Pattern**: Diferentes abilities = diferentes estrategias
- **Composite Pattern**: Actor compone múltiples abilities
- **Fluent Interface**: WhoCan() retorna Actor para chaining
- **Generic Constraints**: Using<T> where T : IAbility

---

**Autor**: QuantumTestSuite Team  
**Revisores**: Pending  
**Última Actualización**: 2024-01-15
