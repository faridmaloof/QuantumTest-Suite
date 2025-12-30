using System;
using System.Text;
using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using QuantumTestSuite.Core.Utilities;
using Reqnroll;

namespace QuantumTestSuite.Tests.StepBindings.UnitFeatures;

[Binding]
[AllureParentSuite("Unit Features")]
[AllureSuite("Utilities")]
[AllureSubSuite("Helper Functions")]
public class UtilitiesStepBindings
{
    private readonly ScenarioContext _scenarioContext;
    private Func<Task>? _testAction;
    private int _executionCount = 0;
    private Exception? _caughtException;
    private bool _actionResult;

    public UtilitiesStepBindings(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"una acción que siempre tiene éxito")]
    [AllureStep("Crear acción exitosa")]
    public void GivenAccionExitosa()
    {
        _executionCount = 0;
        _testAction = () =>
        {
            _executionCount++;
            return Task.CompletedTask;
        };
    }

    [When(@"se ejecuta con RetryHelper con maxAttempts=(\d+)")]
    [AllureStep("Ejecutar con RetryHelper (maxAttempts={maxAttempts})")]
    public async Task WhenSeEjecutaConRetryHelper(int maxAttempts)
    {
        try
        {
            await RetryHelper.ExecuteAsync(_testAction!, maxAttempts);
            AllureApi.AddAttachment("Resultado", "text/plain", Encoding.UTF8.GetBytes("Ejecución exitosa"), ".txt");
        }
        catch (Exception ex)
        {
            _caughtException = ex;
            AllureApi.AddAttachment("Excepción capturada", "text/plain", Encoding.UTF8.GetBytes(ex.Message), ".txt");
        }
    }

    [Then(@"la acción debe ejecutarse exactamente (\d+) vez")]
    [Then(@"la acción debe ejecutarse exactamente (\d+) veces")]
    [AllureStep("Verificar ejecuciones = {expectedCount}")]
    public void ThenAccionEjecutadaVeces(int expectedCount)
    {
        Assert.That(_executionCount, Is.EqualTo(expectedCount),
            $"La acción debió ejecutarse {expectedCount} veces pero se ejecutó {_executionCount}");
        
        AllureApi.AddAttachment("Conteo ejecuciones", "text/plain",
            Encoding.UTF8.GetBytes($"Esperado: {expectedCount}, Real: {_executionCount}"), ".txt");
    }

    [Then(@"debe retornar resultado exitoso")]
    [AllureStep("Verificar resultado exitoso")]
    public void ThenDebeRetornarExitoso()
    {
        Assert.That(_caughtException, Is.Null,
            "No debe haber excepción, pero se capturó: " + _caughtException?.Message);
    }

    [Given(@"una acción que siempre falla")]
    [AllureStep("Crear acción que siempre falla")]
    public void GivenAccionQueSiempreFalla()
    {
        _executionCount = 0;
        _testAction = () =>
        {
            _executionCount++;
            throw new InvalidOperationException("Acción diseñada para fallar");
        };
    }

    [Then(@"debe lanzar excepción después del último intento")]
    [AllureStep("Verificar excepción lanzada")]
    public void ThenDebeLanzarExcepcion()
    {
        Assert.That(_caughtException, Is.Not.Null,
            "Debería haber una excepción capturada");
        Assert.That(_caughtException, Is.TypeOf<InvalidOperationException>());
    }

    [Given(@"una acción que falla (\d+) veces y luego tiene éxito")]
    [AllureStep("Crear acción que falla {failCount} veces")]
    public void GivenAccionQueFallaVecesYLuegoExito(int failCount)
    {
        _executionCount = 0;
        _testAction = () =>
        {
            _executionCount++;
            if (_executionCount <= failCount)
            {
                throw new InvalidOperationException($"Intento {_executionCount} falla");
            }
            return Task.CompletedTask;
        };
    }

    [Then(@"TestTimeouts\.Short debe ser menor a TestTimeouts\.Default")]
    [AllureStep("Verificar Short < Default")]
    public void ThenShortMenorQueDefault()
    {
        Assert.That(TestTimeouts.Short, Is.LessThan(TestTimeouts.Default),
            $"Short ({TestTimeouts.Short}ms) debe ser menor que Default ({TestTimeouts.Default}ms)");
        
        AllureApi.AddAttachment("Timeouts", "text/plain",
            Encoding.UTF8.GetBytes($"Short: {TestTimeouts.Short}ms\nDefault: {TestTimeouts.Default}ms"), ".txt");
    }

    [Then(@"TestTimeouts\.Default debe ser menor a TestTimeouts\.Long")]
    [AllureStep("Verificar Default < Long")]
    public void ThenDefaultMenorQueLong()
    {
        Assert.That(TestTimeouts.Default, Is.LessThan(TestTimeouts.Long),
            $"Default ({TestTimeouts.Default}ms) debe ser menor que Long ({TestTimeouts.Long}ms)");
    }

    [Then(@"TestTimeouts\.Extended debe ser el mayor timeout")]
    [AllureStep("Verificar Extended es el mayor")]
    public void ThenExtendedEsMayor()
    {
        Assert.That(TestTimeouts.Extended, Is.GreaterThan(TestTimeouts.Long),
            $"Extended ({TestTimeouts.Extended}ms) debe ser mayor que Long ({TestTimeouts.Long}ms)");
        
        AllureApi.AddAttachment("Jerarquía Timeouts", "text/plain",
            Encoding.UTF8.GetBytes($"Short: {TestTimeouts.Short}ms\n" +
            $"Default: {TestTimeouts.Default}ms\n" +
            $"Long: {TestTimeouts.Long}ms\n" +
            $"Extended: {TestTimeouts.Extended}ms"), ".txt");
    }

    [Given(@"una condición que se cumple después de (\d+) segundos")]
    [AllureStep("Crear condición que se cumple después de {seconds}s")]
    public void GivenCondicionCumpleDespues(int seconds)
    {
        var startTime = DateTime.Now;
        _scenarioContext["ConditionFunc"] = new Func<bool>(() =>
        {
            return (DateTime.Now - startTime).TotalSeconds >= seconds;
        });
        
        AllureApi.AddAttachment("Condición configurada", "text/plain",
            Encoding.UTF8.GetBytes($"Se cumplirá después de {seconds} segundos"), ".txt");
    }

    [When(@"se usa WaitHelper con timeout de (\d+) segundos")]
    [AllureStep("Ejecutar WaitHelper (timeout={timeout}s)")]
    public async Task WhenSeUsaWaitHelper(int timeout)
    {
        try
        {
            var condition = (Func<bool>)_scenarioContext["ConditionFunc"];
            _actionResult = await WaitHelper.WaitForConditionAsync(
                condition, 
                TimeSpan.FromSeconds(timeout));
            
            AllureApi.AddAttachment("Resultado WaitHelper", "text/plain", 
                Encoding.UTF8.GetBytes($"Condición cumplida: {_actionResult}"), ".txt");
        }
        catch (Exception ex)
        {
            _caughtException = ex;
            AllureApi.AddAttachment("Excepción", "text/plain", Encoding.UTF8.GetBytes(ex.Message), ".txt");
        }
    }

    [Then(@"debe retornar true cuando la condición se cumpla")]
    [AllureStep("Verificar retorno = true")]
    public void ThenDebeRetornarTrue()
    {
        Assert.That(_actionResult, Is.True,
            "WaitHelper debe retornar true cuando la condición se cumple");
    }

    [Then(@"debe completar antes del timeout")]
    [AllureStep("Verificar que completó antes del timeout")]
    public void ThenDebeCompletarAntesTimeout()
    {
        Assert.That(_caughtException, Is.Null,
            "No debe haber TimeoutException");
    }

    [Given(@"una condición que nunca se cumple")]
    [AllureStep("Crear condición que nunca se cumple")]
    public void GivenCondicionNuncaCumple()
    {
        _scenarioContext["ConditionFunc"] = new Func<bool>(() => false);
        AllureApi.AddAttachment("Condición", "text/plain", Encoding.UTF8.GetBytes("Siempre retorna false"), ".txt");
    }

    [Then(@"debe lanzar TimeoutException después de (\d+) segundos")]
    [AllureStep("Verificar TimeoutException después de {seconds}s")]
    public void ThenDebeLanzarTimeoutException(int seconds)
    {
        Assert.That(_caughtException, Is.Not.Null,
            "Debe haber capturado una excepción");
        Assert.That(_caughtException, Is.TypeOf<TimeoutException>(),
            $"Debe ser TimeoutException, pero es {_caughtException?.GetType().Name}");
    }
}
