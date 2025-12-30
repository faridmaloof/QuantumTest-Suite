using Microsoft.Playwright;
using NUnit.Framework;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Core.Context;
using QuantumTestSuite.Core.Models;
using QuantumTestSuite.UI.Drivers;
using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Abilities;
using QuantumTestSuite.UI.Screenplay.Actors;
using QuantumTestSuite.UI.Screenplay.Tasks;
using System.Data;
using Reqnroll;

namespace QuantumTestSuite.Tests.StepBindings;

[Binding]
public class AbilitiesDemoStepBindings
{
    private readonly ScenarioContext _scenarioContext;
    private readonly AppSettings _settings;
    private UiTestContext Context => _scenarioContext.Get<UiTestContext>();
    private Actor? _actor;

    public AbilitiesDemoStepBindings(ScenarioContext scenarioContext, AppSettings settings)
    {
        _scenarioContext = scenarioContext;
        _settings = settings;
    }

    [Given(@"el Actor tiene las habilidades necesarias")]
    public async Task GivenActorTieneHabilidades()
    {
        var (playwright, browser, context, page) = await PlaywrightDriver.LaunchAsync(_settings);
        
        Context.Playwright = playwright;
        Context.Browser = browser;
        Context.BrowserContext = context;
        Context.Page = page;

        // Crear Actor con Abilities
        _actor = new Actor("TestUser", page)
            .WhoCan(new RememberData())
            .WhoCan(new ReadConfiguration(_settings));

        // Nota: Para AccessDatabase y CallApiEndpoint, se necesitarían conexiones reales
        // Aquí mostramos cómo se registrarían:
        // .WhoCan(new AccessDatabase(dbConnection, new DatabaseConfig()))
        // .WhoCan(new CallApiEndpoint(apiContext, _settings))
    }

    [Given(@"obtengo usuarios válidos desde la base de datos usando AccessDatabase")]
    public async Task GivenObtenerUsuariosDesdeBD()
    {
        // En un escenario real con BD:
        // var users = await _actor!.Using<AccessDatabase>().GetActiveUsersAsync();
        // _actor.Using<RememberData>().Remember("validUsers", users);
        // _actor.Using<RememberData>().Remember("selectedUser", users.First());

        // Para demo sin BD, usamos datos mock:
        var mockUsers = new List<User>
        {
            new()
            {
                Id = 1,
                Username = "standard_user",
                Password = "secret_sauce",
                FirstName = "Test",
                LastName = "User",
                Active = true,
                Email = "test@example.com"
            }
        };

        _actor!.Using<RememberData>().Remember("validUsers", mockUsers);
        _actor.Using<RememberData>().Remember("selectedUser", mockUsers.First());
    }

    [When(@"inicio sesión con el usuario recordado usando RememberData")]
    public async Task WhenIniciarSesionConUsuarioRecordado()
    {
        // Recuperar usuario desde RememberData ability
        var user = _actor!.Using<RememberData>().Recall<User>("selectedUser");
        
        Assert.That(user, Is.Not.Null, "Usuario no debe ser null");

        // Usar el usuario para login
        var loginPage = new SauceDemoLoginPage(Context.Page!, _settings);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(user.Username, user.Password);
    }

    [Given(@"el Actor lee la configuración usando ReadConfiguration ability")]
    public void GivenActorLeeConfiguracion()
    {
        // Obtener configuración usando ReadConfiguration ability
        var baseUrl = _actor!.Using<ReadConfiguration>().GetBaseUrl();
        var browser = _actor.Using<ReadConfiguration>().GetBrowser();
        var env = _actor.Using<ReadConfiguration>().GetEnvironment();

        // Recordar para uso posterior
        _actor.Using<RememberData>().Remember("baseUrl", baseUrl);
        _actor.Using<RememberData>().Remember("environment", env);

        Console.WriteLine($"Configuración cargada - Env: {env}, Browser: {browser}, BaseURL: {baseUrl}");
    }

    [When(@"navego a la URL base configurada")]
    public async Task WhenNavegoAUrlBase()
    {
        var baseUrl = _actor!.Using<RememberData>().Recall<string>("baseUrl");
        await Context.Page!.GotoAsync(baseUrl!);
    }

    [Then(@"debo estar en la página correcta según el entorno")]
    public void ThenDeboEstarEnPaginaCorrecta()
    {
        var currentUrl = Context.Page!.Url;
        var expectedUrl = _actor!.Using<ReadConfiguration>().GetBaseUrl();
        
        Assert.That(currentUrl, Does.Contain(expectedUrl),
            $"URL actual {currentUrl} debe contener URL esperada {expectedUrl}");
    }

    [AfterScenario]
    public async Task CleanupActor()
    {
        if (_actor != null)
        {
            await _actor.CleanupAsync();
        }
    }
}
