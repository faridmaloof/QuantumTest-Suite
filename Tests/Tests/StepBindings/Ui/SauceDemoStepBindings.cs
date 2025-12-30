using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Tests.StepBindings.Base;
using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Tasks;
using Reqnroll;

namespace QuantumTestSuite.Tests.StepBindings.Ui;

/// <summary>
/// Step bindings for SauceDemo login tests
/// </summary>
[Binding]
[AllureParentSuite("UI Tests")]
[AllureSuite("Authentication")]
[AllureFeature("SauceDemo Login")]
public class SauceDemoStepBindings : UiStepBindingsBase
{
    public SauceDemoStepBindings(
        ScenarioContext scenarioContext,
        AppSettings settings)
        : base(scenarioContext, settings)
    {
    }

    [Given(@"el usuario está en la página de SauceDemo")]
    public async Task GivenElUsuarioEstaEnSauceDemo()
    {
        await ExecuteGivenAsync(async () =>
        {
            await EnsureActorAsync("QA");
            var page = new SauceDemoLoginPage(Context.Page!);
            await page.NavigateAsync();
        });
    }

    [When(@"ingresa credenciales válidas")]
    public async Task WhenIngresaCredencialesValidas()
    {
        await ExecuteWhenAsync(async () =>
        {
            await EnsureActorAsync("QA");
            await Actor!.AttemptsTo(new LoginToSauceDemo(
                Settings.Users.SauceDemo.Username,
                Settings.Users.SauceDemo.Password));
        });
    }

    [Then(@"debe ver la página de inventario")]
    public async Task ThenDebeVerLaPaginaDeInventario()
    {
        await ExecuteThenAsync(async () =>
        {
            var page = new SauceDemoLoginPage(Context.Page!);
            var visible = await page.IsInventoryVisibleAsync();
            Assert.That(visible, Is.True, "Inventory page should be visible after successful login");
        });
    }
}
