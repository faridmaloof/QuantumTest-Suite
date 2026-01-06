using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.Framework.Core.Config;
using QuantumTestSuite.Framework.Core.Context;
using QuantumTestSuite.StepBindings.Base;
using Reqnroll;

namespace QuantumTestSuite.StepBindings.Api;

/// <summary>
/// Common step bindings for API assertions (used across all API tests)
/// </summary>
[Binding]
[AllureParentSuite("API Tests")]
public class ApiCommonStepBindings : ApiStepBindingsBase
{
    public ApiCommonStepBindings(
        ScenarioContext scenarioContext,
        AppSettings settings)
        : base(scenarioContext, settings)
    {
    }

    [Then(@"el status debe ser (.*)")]
    [Then(@"the status must be (.*)")]
    [Then(@"o status deve ser (.*)")]
    public void ThenElStatusDebeSer(int expectedStatus)
    {
        int? actualStatus = null;

        // Check which response type is available
        if (Context.HttpBinResponse != null)
            actualStatus = Context.HttpBinResponse.StatusCode;
        else if (Context.BookingResponse != null)
            actualStatus = Context.BookingResponse.StatusCode;
        else if (Context.GitHubResponse != null)
            actualStatus = Context.GitHubResponse.StatusCode;

        Assert.That(actualStatus, Is.EqualTo(expectedStatus),
            $"Expected status {expectedStatus} but got {actualStatus}");
    }
}
