using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Core.Context;
using QuantumTestSuite.Tests.StepBindings.Base;
using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Tasks;
using Reqnroll;

namespace QuantumTestSuite.Tests.StepBindings.Ui;

/// <summary>
/// Step bindings for GitHub search UI tests (E2E with API)
/// </summary>
[Binding]
[AllureParentSuite("E2E Tests")]
[AllureSuite("Hybrid Flows")]
[AllureFeature("UI-API Integration")]
public class GitHubUiStepBindings : UiStepBindingsBase
{
    public GitHubUiStepBindings(
        ScenarioContext scenarioContext,
        AppSettings settings)
        : base(scenarioContext, settings)
    {
    }

    [When(@"busca el usuario en UI")]
    [When(@"searches the user in UI")]
    [When(@"busca o usuario na UI")]
    public async Task WhenBuscaUsuarioEnUi()
    {
        await ExecuteWhenAsync(async () =>
        {
            await EnsureActorAsync("QA");

            // Get GitHub user from API context (cross-context usage for UI+API scenarios)
            var apiContext = ScenarioContext.Get<ApiTestContext>();
            var username = apiContext.GitHubUser!;

            await Actor!.AttemptsTo(new SearchGitHubUser(Settings.Apis.GhUsersSearchUi, username));
        });
    }

    [Then(@"el resultado muestra el usuario")]
    [Then(@"the result shows the user")]
    [Then(@"o resultado mostra o usuario")]
    public async Task ThenElResultadoMuestraUsuario()
    {
        await ExecuteThenAsync(async () =>
        {
            // Get GitHub user from API context
            var apiContext = ScenarioContext.Get<ApiTestContext>();
            var username = apiContext.GitHubUser!;

            var page = new GhUsersSearchPage(Context.Page!);
            var hasUser = await page.HasUserAsync(username);
            Assert.That(hasUser, Is.True, $"User {username} should be displayed in search results");
        });
    }
}
