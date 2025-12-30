using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Core.Services;
using QuantumTestSuite.Tests.StepBindings.Base;
using Reqnroll;

namespace QuantumTestSuite.Tests.StepBindings.Api;

/// <summary>
/// Step bindings for GitHub API tests
/// </summary>
[Binding]
[AllureParentSuite("API Tests")]
[AllureSuite("GitHub")]
public class GitHubStepBindings : ApiStepBindingsBase
{
    private readonly IGitHubService _gitHubService;

    public GitHubStepBindings(
        ScenarioContext scenarioContext,
        AppSettings settings,
        IGitHubService gitHubService)
        : base(scenarioContext, settings)
    {
        _gitHubService = gitHubService;
    }

    [Given(@"un usuario de GitHub llamado '(.*)' existe via API")]
    [Given(@"a GitHub user named '(.*)' exists via API")]
    [Given(@"um usuario do GitHub chamado '(.*)' existe via API")]
    public async Task GivenUnUsuarioDeGitHubExiste(string username)
    {
        Context.GitHubUser = username;

        var response = await ExecuteApiCallAsync(
            "GET",
            $"{Settings.Apis.GitHub}/search/users?q={username}",
            () => _gitHubService.SearchUserAsync(username));

        Context.GitHubResponse = response;
    }
}
