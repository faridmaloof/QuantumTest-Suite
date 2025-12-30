using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using Microsoft.Playwright;
using Moq;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Core.Models;
using QuantumTestSuite.UI.Screenplay.Abilities;
using QuantumTestSuite.UI.Screenplay.Actors;
using Reqnroll;
using System.Text;

namespace QuantumTestSuite.Tests.StepBindings.UnitFeatures;

[Binding]
[AllureParentSuite("Unit Features")]
[AllureSuite("Abilities System")]
[AllureSubSuite("Screenplay Pattern")]
public class AbilitiesTestsStepBindings
{
    private readonly ScenarioContext _scenarioContext;
    private Actor? _actor;
    private Exception? _caughtException;
    private IAbility? _retrievedAbility;

    public AbilitiesTestsStepBindings(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"tengo un Actor con una página mock")]
    [AllureStep("Crear Actor con página mock")]
    public void GivenActorConPaginaMock()
    {
        var pageMock = new Mock<IPage>();
        _actor = new Actor("TestActor", pageMock.Object);
        
        Assert.That(_actor, Is.Not.Null);
        Assert.That(_actor.Name, Is.EqualTo("TestActor"));
    }

    [Given(@"el Actor tiene la ability RememberData")]
    [AllureStep("Asignar RememberData ability al Actor")]
    public void GivenActorTieneRememberData()
    {
        _actor = _actor!.WhoCan(new RememberData());
    }

    [Given(@"el Actor NO tiene la ability AccessDatabase")]
    [AllureStep("Actor NO tiene AccessDatabase")]
    public void GivenActorNoTieneAccessDatabase()
    {
        // No asignamos la ability, simplemente documentamos
        AllureApi.AddAttachment("Estado", "text/plain", Encoding.UTF8.GetBytes("Actor no tiene AccessDatabase ability"), ".txt");
    }

    [Given(@"el Actor tiene la ability ReadConfiguration")]
    [AllureStep("Asignar ReadConfiguration ability")]
    public void GivenActorTieneReadConfiguration()
    {
        var settings = ConfigManager.Settings;
        _actor = _actor!.WhoCan(new ReadConfiguration(settings));
    }

    [Given(@"el Actor tiene múltiples Abilities")]
    [AllureStep("Asignar múltiples Abilities")]
    public void GivenActorTieneMultiplesAbilities()
    {
        var settings = ConfigManager.Settings;
        _actor = _actor!
            .WhoCan(new RememberData())
            .WhoCan(new ReadConfiguration(settings));
    }

    [Given(@"almaceno un valor con clave ""(.*)""")]
    [AllureStep("Almacenar valor con clave '{key}'")]
    public void GivenAlmacenarValorConClave(string key)
    {
        _actor!.Using<RememberData>().Remember(key, "test-value");
    }

    [When(@"intento usar la ability RememberData")]
    [AllureStep("Intentar usar RememberData")]
    public void WhenUsarRememberData()
    {
        try
        {
            _retrievedAbility = _actor!.Using<RememberData>();
        }
        catch (Exception ex)
        {
            _caughtException = ex;
        }
    }

    [When(@"intento usar la ability AccessDatabase")]
    [AllureStep("Intentar usar AccessDatabase")]
    public void WhenUsarAccessDatabase()
    {
        try
        {
            _retrievedAbility = _actor!.Using<AccessDatabase>();
        }
        catch (Exception ex)
        {
            _caughtException = ex;
        }
    }

    [When(@"asigno múltiples Abilities al Actor con WhoCan")]
    [AllureStep("Asignar múltiples Abilities")]
    public void WhenAsignarMultiplesAbilities()
    {
        var settings = ConfigManager.Settings;
        
        _actor = _actor!
            .WhoCan(new RememberData())
            .WhoCan(new ReadConfiguration(settings));
    }

    [When(@"almaceno un valor con clave ""(.*)""")]
    [AllureStep("Almacenar valor con clave '{key}'")]
    public void WhenAlmacenarValor(string key)
    {
        _actor!.Using<RememberData>().Remember(key, "stored-value");
    }

    [When(@"almaceno un objeto User con clave ""(.*)""")]
    [AllureStep("Almacenar User con clave '{key}'")]
    public void WhenAlmacenarUser(string key)
    {
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            Active = true
        };
        
        _actor!.Using<RememberData>().Remember(key, user);
    }

    [When(@"olvido la clave ""(.*)""")]
    [AllureStep("Olvidar clave '{key}'")]
    public void WhenOlvidarClave(string key)
    {
        var forgotten = _actor!.Using<RememberData>().Forget(key);
        AllureApi.AddAttachment("Resultado forget", "text/plain", Encoding.UTF8.GetBytes($"Forgotten: {forgotten}"), ".txt");
    }

    [When(@"solicito la URL base")]
    [AllureStep("Solicitar URL base")]
    public void WhenSolicitarUrlBase()
    {
        try
        {
            var baseUrl = _actor!.Using<ReadConfiguration>().GetBaseUrl();
            _scenarioContext["baseUrl"] = baseUrl;
        }
        catch (Exception ex)
        {
            _caughtException = ex;
        }
    }

    [When(@"ejecuto CleanupAsync en el Actor")]
    [AllureStep("Ejecutar CleanupAsync")]
    public async Task WhenEjecutarCleanup()
    {
        await _actor!.CleanupAsync();
    }

    [Then(@"la ability debe estar disponible")]
    [AllureStep("Verificar ability disponible")]
    public void ThenAbilityDisponible()
    {
        Assert.That(_retrievedAbility, Is.Not.Null, "Ability debe estar disponible");
        Assert.That(_retrievedAbility, Is.InstanceOf<RememberData>());
    }

    [Then(@"debe lanzar AbilityNotFoundException")]
    [AllureStep("Verificar AbilityNotFoundException")]
    public void ThenLanzarAbilityNotFoundException()
    {
        Assert.That(_caughtException, Is.Not.Null, "Debe haberse lanzado una excepción");
        Assert.That(_caughtException, Is.TypeOf<AbilityNotFoundException>());
        
        var abilityEx = (AbilityNotFoundException)_caughtException;
        Assert.That(abilityEx.ActorName, Is.EqualTo("TestActor"));
        Assert.That(abilityEx.AbilityType, Is.EqualTo(typeof(AccessDatabase)));
    }

    [Then(@"el Actor debe tener todas las Abilities asignadas")]
    [AllureStep("Verificar múltiples Abilities")]
    public void ThenActorTieneTodasAbilities()
    {
        Assert.That(_actor!.Has<RememberData>(), Is.True, "Debe tener RememberData");
        Assert.That(_actor.Has<ReadConfiguration>(), Is.True, "Debe tener ReadConfiguration");
    }

    [Then(@"puedo recuperar el valor usando la misma clave")]
    [AllureStep("Recuperar valor almacenado")]
    public void ThenRecuperarValor()
    {
        var value = _actor!.Using<RememberData>().Recall<string>("testKey");
        Assert.That(value, Is.Not.Null);
        Assert.That(value, Is.EqualTo("stored-value"));
    }

    [Then(@"puedo recuperar el User usando la misma clave")]
    [AllureStep("Recuperar User almacenado")]
    public void ThenRecuperarUser()
    {
        var user = _actor!.Using<RememberData>().Recall<User>("user");
        
        Assert.That(user, Is.Not.Null);
        Assert.That(user.Username, Is.EqualTo("testuser"));
        Assert.That(user.Email, Is.EqualTo("test@example.com"));
    }

    [Then(@"debe lanzar InvalidCastException si intento recuperarlo como string")]
    [AllureStep("Verificar InvalidCastException en type mismatch")]
    public void ThenLanzarInvalidCastException()
    {
        Assert.Throws<InvalidCastException>(() =>
        {
            var wrongType = _actor!.Using<RememberData>().Recall<string>("user");
        });
    }

    [Then(@"la clave ""(.*)"" no debe existir en memoria")]
    [AllureStep("Verificar clave '{key}' no existe")]
    public void ThenClaveNoExiste(string key)
    {
        var exists = _actor!.Using<RememberData>().Has(key);
        Assert.That(exists, Is.False, $"Clave '{key}' no debe existir");
    }

    [Then(@"debo obtener la URL configurada")]
    [AllureStep("Verificar URL configurada")]
    public void ThenObtenerUrlConfigurada()
    {
        Assert.That(_caughtException, Is.Null, "No debe haber excepción");
        
        var baseUrl = (string)_scenarioContext["baseUrl"];
        Assert.That(baseUrl, Is.Not.Null.And.Not.Empty);
        
        AllureApi.AddAttachment("Base URL", "text/plain", Encoding.UTF8.GetBytes(baseUrl), ".txt");
    }

    [Then(@"todas las Abilities deben limpiar sus recursos")]
    [AllureStep("Verificar cleanup de Abilities")]
    public void ThenAbilitiesLimpiaronRecursos()
    {
        // Después del cleanup, las abilities siguen existiendo pero están limpias
        // RememberData debería haber limpiado su memoria
        var rememberData = _actor!.Using<RememberData>();
        Assert.That(rememberData.Keys, Is.Empty, "RememberData debe estar vacía después del cleanup");
    }
}
