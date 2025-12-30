using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Tests.StepBindings.Base;
using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Tasks;
using Reqnroll;

namespace QuantumTestSuite.Tests.StepBindings.Ui;

/// <summary>
/// Step bindings for UltimateQA form tests
/// </summary>
[Binding]
[AllureParentSuite("UI Tests")]
[AllureSuite("Forms")]
[AllureFeature("Ultimate QA Form")]
public class UltimateQaStepBindings : UiStepBindingsBase
{
    public UltimateQaStepBindings(
        ScenarioContext scenarioContext,
        AppSettings settings)
        : base(scenarioContext, settings)
    {
    }

    [Given(@"el usuario abre el formulario de Ultimate QA")]
    public async Task GivenElUsuarioAbreUltimateQa()
    {
        await ExecuteGivenAsync(async () =>
        {
            await EnsureActorAsync("QA");
        });
    }

    [When(@"completa el formulario de contacto")]
    public async Task WhenCompletaElFormulario()
    {
        await ExecuteWhenAsync(async () =>
        {
            await EnsureActorAsync("QA");
            await Actor!.AttemptsTo(new SubmitUltimateQaForm(
                "Quantum",
                "qa@example.com",
                "Formulario de prueba"));
        });
    }

    [Then(@"visualiza confirmación de envío")]
    public async Task ThenVisualizaConfirmacion()
    {
        await ExecuteThenAsync(async () =>
        {
            var page = new UltimateQaFormPage(Context.Page!);
            var success = await page.IsSuccessVisibleAsync();
            Assert.That(success, Is.True, "Success confirmation should be visible after form submission");
        });
    }
}
