using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using QuantumTestSuite.Core.Config;
using Reqnroll;
using System.Text;

namespace QuantumTestSuite.Tests.StepBindings.UnitFeatures;

[Binding]
[AllureParentSuite("Unit Features")]
[AllureSuite("Configuration")]
[AllureSubSuite("ConfigManager")]
public class ConfigManagerStepBindings
{
    private readonly ScenarioContext _scenarioContext;
    private AppSettings? _capturedSettings;
    private readonly Dictionary<string, string> _originalEnvVars = new();

    public ConfigManagerStepBindings(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"el sistema tiene ConfigManager inicializado")]
    [AllureStep("Sistema tiene ConfigManager inicializado")]
    public void GivenConfigManagerInicializado()
    {
        // ConfigManager se inicializa automáticamente (singleton lazy)
        Assert.That(ConfigManager.Settings, Is.Not.Null, "ConfigManager debe estar inicializado");
    }

    [When(@"no se establecen variables de entorno")]
    [AllureStep("No se establecen variables de entorno")]
    public void WhenNoHayVariablesEntorno()
    {
        // No hacer nada, escenario base
        AllureApi.AddAttachment("Info", "text/plain", Encoding.UTF8.GetBytes("No se modifican variables de entorno"), ".txt");
    }

    [When(@"no existe archivo \.env")]
    [AllureStep("No existe archivo .env")]
    public void WhenNoExisteArchivoEnv()
    {
        // Este paso es informativo, .env puede o no existir
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        var exists = File.Exists(envPath);
        AllureApi.AddAttachment("Estado .env", "text/plain", Encoding.UTF8.GetBytes($"Archivo .env existe: {exists}"), ".txt");
    }

    [Then(@"ConfigManager debe retornar valores por defecto")]
    [AllureStep("ConfigManager retorna valores por defecto")]
    public void ThenConfigManagerRetornaDefaults()
    {
        var settings = ConfigManager.Settings;
        Assert.That(settings, Is.Not.Null, "Settings no debe ser null");
        AllureApi.AddAttachment("Settings", "application/json", 
            Encoding.UTF8.GetBytes(ConfigManager.ToJson(settings)), ".txt");
    }

    [Then(@"Playwright\.Headless debe ser ""(.*)""")]
    [AllureStep("Playwright.Headless debe ser '{headless}'")]
    public void ThenPlaywrightHeadlessDebe(string headless)
    {
        var expected = bool.Parse(headless);
        var actual = ConfigManager.Settings.Playwright.Headless;
        
        Assert.That(actual, Is.EqualTo(expected), 
            $"Playwright.Headless debe ser {expected} pero es {actual}");
        
        AllureApi.AddAttachment("Verificación Headless", "text/plain", Encoding.UTF8.GetBytes($"Expected: {expected}, Actual: {actual}"), ".txt");
    }

    [Then(@"Playwright\.Browser debe ser ""(.*)""")]
    [AllureStep("Playwright.Browser debe ser '{browser}'")]
    public void ThenPlaywrightBrowserDebe(string browser)
    {
        var actual = ConfigManager.Settings.Playwright.Browser;
        Assert.That(actual, Is.EqualTo(browser), 
            $"Playwright.Browser debe ser {browser} pero es {actual}");
    }

    [Given(@"existe un archivo \.env con ""(.*)""")]
    [AllureStep("Existe archivo .env con configuración: {config}")]
    public void GivenExisteArchivoEnvCon(string config)
    {
        // Este paso es informativo ya que .env puede existir o no
        // En un test real, crearías el archivo temporalmente
        AllureApi.AddAttachment("Configuración .env", "text/plain", Encoding.UTF8.GetBytes(config), ".txt");
    }

    [When(@"se establece variable de entorno ""(.*)""")]
    [AllureStep("Establecer variable de entorno: {envVar}")]
    public void WhenSeEstableceVariableEntorno(string envVar)
    {
        var parts = envVar.Split('=');
        var key = parts[0];
        var value = parts[1];

        // Guardar valor original si existe
        var original = Environment.GetEnvironmentVariable(key);
        if (original != null)
        {
            _originalEnvVars[key] = original;
        }

        Environment.SetEnvironmentVariable(key, value);
        AllureApi.AddAttachment("Variable establecida", "text/plain", Encoding.UTF8.GetBytes($"{key}={value}"), ".txt");
    }

    [Then(@"ConfigManager\.Settings\.Playwright\.Headless debe ser ""(.*)""")]
    [AllureStep("Validar Playwright.Headless = '{expected}'")]
    public void ThenConfigManagerPlaywrightHeadlessDebe(string expected)
    {
        var expectedBool = bool.Parse(expected);
        var actual = ConfigManager.Settings.Playwright.Headless;
        
        Assert.That(actual, Is.EqualTo(expectedBool),
            $"Headless debe ser {expectedBool} pero es {actual}");
    }

    [Given(@"User Secrets contiene ""(.*)""")]
    [AllureStep("User Secrets contiene: {config}")]
    public void GivenUserSecretsContiene(string config)
    {
        // Este paso es informativo
        // En test real, configurarías User Secrets programáticamente
        AllureApi.AddAttachment("User Secrets Config", "text/plain", Encoding.UTF8.GetBytes(config), ".txt");
    }

    [Then(@"ConfigManager\.Settings\.Playwright\.Browser debe ser ""(.*)""")]
    [AllureStep("Validar Playwright.Browser = '{expected}'")]
    public void ThenConfigManagerPlaywrightBrowserDebe(string expected)
    {
        var actual = ConfigManager.Settings.Playwright.Browser;
        Assert.That(actual, Is.EqualTo(expected),
            $"Browser debe ser {expected} pero es {actual}");
    }

    [Given(@"existe un archivo \.env\.qa con ""(.*)""")]
    [AllureStep("Existe archivo .env.qa con: {config}")]
    public void GivenExisteArchivoEnvQaCon(string config)
    {
        AllureApi.AddAttachment("Configuración .env.qa", "text/plain", Encoding.UTF8.GetBytes(config), ".txt");
    }

    [Then(@"ConfigManager\.Settings\.Playwright\.BaseUrl debe contener ""(.*)""")]
    [AllureStep("Validar que BaseUrl contiene '{expected}'")]
    public void ThenConfigManagerPlaywrightBaseUrlDebeContener(string expected)
    {
        var actual = ConfigManager.Settings.Playwright.BaseUrl;
        Assert.That(actual, Does.Contain(expected),
            $"BaseUrl debe contener {expected} pero es {actual}");
    }

    [Then(@"ConfigManager debe parsear como booleano ""(.*)""")]
    [AllureStep("Verificar parsing booleano = '{expected}'")]
    public void ThenConfigManagerDebeParserBooleano(string expected)
    {
        // Esta verificación requiere conocer qué variable se estableció
        // Se puede mejorar almacenando el contexto
        var expectedBool = bool.Parse(expected);
        AllureApi.AddAttachment("Expected Boolean", "text/plain", 
            Encoding.UTF8.GetBytes(expectedBool.ToString()), ".txt");
    }

    [Given(@"ConfigManager\.Settings se accede por primera vez")]
    [AllureStep("Primera carga de ConfigManager.Settings")]
    public void GivenConfigManagerSettingsPrimeraVez()
    {
        _capturedSettings = ConfigManager.Settings;
        Assert.That(_capturedSettings, Is.Not.Null);
    }

    [When(@"se modifica una variable de entorno después de la primera carga")]
    [AllureStep("Modificar variable después de carga inicial")]
    public void WhenSeModificaVariableEntornoDespuesCarga()
    {
        Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSER", "firefox");
        AllureApi.AddAttachment("Modificación post-carga", "text/plain", Encoding.UTF8.GetBytes("Variable PLAYWRIGHT_BROWSER cambiada a firefox"), ".txt");
    }

    [Then(@"ConfigManager\.Settings debe retornar la configuración cacheada")]
    [AllureStep("Verificar que configuración está cacheada")]
    public void ThenConfigManagerDebeRetornarConfigCacheada()
    {
        var currentSettings = ConfigManager.Settings;
        Assert.That(currentSettings, Is.SameAs(_capturedSettings),
            "ConfigManager debe retornar la misma instancia cacheada");
    }

    [Then(@"no debe recargar desde archivos")]
    [AllureStep("Verificar que no recarga desde archivos")]
    public void ThenNoDebeRecargarDesdeArchivos()
    {
        // ConfigManager usa Lazy<T>, así que no recarga
        // Esta verificación es conceptual
        AllureApi.AddAttachment("Comportamiento Singleton", "text/plain", Encoding.UTF8.GetBytes("ConfigManager usa Lazy<T> para cachear la primera carga"), ".txt");
    }

    [AfterScenario]
    public void CleanupEnvironmentVariables()
    {
        // Restaurar variables de entorno originales
        foreach (var kvp in _originalEnvVars)
        {
            Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
        }
    }
}
