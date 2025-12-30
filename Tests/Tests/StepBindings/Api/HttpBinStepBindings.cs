using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Core.Services;
using QuantumTestSuite.Tests.StepBindings.Base;
using Reqnroll;

namespace QuantumTestSuite.Tests.StepBindings.Api;

/// <summary>
/// Step bindings for HttpBin API tests
/// </summary>
[Binding]
[AllureParentSuite("API Tests")]
[AllureSuite("HttpBin")]
public class HttpBinStepBindings : ApiStepBindingsBase
{
    private readonly IHttpBinService _httpBinService;

    public HttpBinStepBindings(
        ScenarioContext scenarioContext,
        AppSettings settings,
        IHttpBinService httpBinService) 
        : base(scenarioContext, settings)
    {
        _httpBinService = httpBinService;
    }

    [Given(@"una llamada a GET \/get")]
    [Given(@"a call is GET \/get")]
    public async Task GivenUnaLlamadaAGet()
    {
        var response = await ExecuteApiCallAsync(
            "GET",
            $"{Settings.Apis.HttpBin}/get",
            () => _httpBinService.GetAsync());

        Context.HttpBinResponse = response;
    }

    [When(@"se ejecuta la petición")]
    [When(@"the request is executed")]
    public Task WhenSeEjecutaLaPeticion()
    {
        // Request already executed in Given step
        return Task.CompletedTask;
    }
}
