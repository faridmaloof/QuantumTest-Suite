using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Tests.StepBindings.Base;
using QuantumTestSuite.API.Clients;
using QuantumTestSuite.API.Questions;
using QuantumTestSuite.API.Models;
using Reqnroll;
using System.Net;

namespace QuantumTestSuite.Tests.StepBindings.Api;

[Binding]
[AllureParentSuite("API Tests")]
[AllureSuite("Pokemon API")]
[AllureFeature("Pokemon Data")]
public class PokemonApiStepBindings : ApiStepBindingsBase
{
    private readonly PokeApiClient _pokeApiClient;
    private HttpResponseMessage? _lastResponse;
    private Pokemon? _lastPokemon;

    public PokemonApiStepBindings(
        ScenarioContext scenarioContext,
        AppSettings settings)
        : base(scenarioContext, settings)
    {
        _pokeApiClient = new PokeApiClient();
    }

    [When(@"I request pokemon with ID (.*)")]
    public async Task WhenIRequestPokemonWithID(int pokemonId)
    {
        var result = await _pokeApiClient.GetPokemonByIdAsync(pokemonId);
        _lastPokemon = result.Data;
        _lastResponse = result.Response;
    }

    [When(@"I request pokemon ""(.*)""")]
    public async Task WhenIRequestPokemon(string pokemonName)
    {
        var result = await _pokeApiClient.GetPokemonByNameAsync(pokemonName);
        _lastPokemon = result.Data;
        _lastResponse = result.Response;
    }

    [Then(@"the response status should be (.*)")]
    public async Task ThenTheResponseStatusShouldBe(int expectedStatus)
    {
        Assert.That(_lastResponse, Is.Not.Null, "No response received");
        Assert.That((int)_lastResponse!.StatusCode, Is.EqualTo(expectedStatus),
            $"Expected status {expectedStatus} but got {(int)_lastResponse.StatusCode}");
        
        await Task.CompletedTask;
    }

    [Then(@"the pokemon name should be ""(.*)""")]
    public async Task ThenThePokemonNameShouldBe(string expectedName)
    {
        Assert.That(_lastPokemon, Is.Not.Null, "No pokemon data received");
        Assert.That(_lastPokemon!.Name, Is.EqualTo(expectedName),
            $"Expected name '{expectedName}' but got '{_lastPokemon.Name}'");
        
        await Task.CompletedTask;
    }

    [Then(@"the pokemon should have ""(.*)"" type")]
    public async Task ThenThePokemonShouldHaveType(string expectedType)
    {
        Assert.That(_lastPokemon, Is.Not.Null, "No pokemon data received");
        
        var hasType = _lastPokemon!.Types.Any(t => t.Type.Name == expectedType);
        Assert.That(hasType, Is.True,
            $"Expected pokemon to have type '{expectedType}' but types are: {string.Join(", ", _lastPokemon.Types.Select(t => t.Type.Name))}");
        
        await Task.CompletedTask;
    }

    [Then(@"the pokemon ID should be (.*)")]
    public async Task ThenThePokemonIDShouldBe(int expectedId)
    {
        Assert.That(_lastPokemon, Is.Not.Null, "No pokemon data received");
        Assert.That(_lastPokemon!.Id, Is.EqualTo(expectedId),
            $"Expected ID {expectedId} but got {_lastPokemon.Id}");
        
        await Task.CompletedTask;
    }

    [Then(@"the pokemon should have (.*) types")]
    public async Task ThenThePokemonShouldHaveTypes(int expectedCount)
    {
        Assert.That(_lastPokemon, Is.Not.Null, "No pokemon data received");
        Assert.That(_lastPokemon!.Types.Count, Is.EqualTo(expectedCount),
            $"Expected {expectedCount} types but got {_lastPokemon.Types.Count}");
        
        await Task.CompletedTask;
    }

    [Then(@"the pokemon should have the ability ""(.*)""")]
    public async Task ThenThePokemonShouldHaveTheAbility(string expectedAbility)
    {
        Assert.That(_lastPokemon, Is.Not.Null, "No pokemon data received");
        
        var hasAbility = _lastPokemon!.Abilities.Any(a => a.Ability.Name == expectedAbility);
        Assert.That(hasAbility, Is.True,
            $"Expected pokemon to have ability '{expectedAbility}' but abilities are: {string.Join(", ", _lastPokemon.Abilities.Select(a => a.Ability.Name))}");
        
        await Task.CompletedTask;
    }

    [Then(@"the pokemon should have at least (.*) ability")]
    [Then(@"the pokemon should have at least (.*) abilities")]
    public async Task ThenThePokemonShouldHaveAtLeastAbilities(int minCount)
    {
        Assert.That(_lastPokemon, Is.Not.Null, "No pokemon data received");
        Assert.That(_lastPokemon!.Abilities.Count, Is.GreaterThanOrEqualTo(minCount),
            $"Expected at least {minCount} abilities but got {_lastPokemon.Abilities.Count}");
        
        await Task.CompletedTask;
    }

    [Then(@"the pokemon should have (.*) stats")]
    public async Task ThenThePokemonShouldHaveStats(int expectedCount)
    {
        Assert.That(_lastPokemon, Is.Not.Null, "No pokemon data received");
        Assert.That(_lastPokemon!.Stats.Count, Is.EqualTo(expectedCount),
            $"Expected {expectedCount} stats but got {_lastPokemon.Stats.Count}");
        
        await Task.CompletedTask;
    }

    [Then(@"the pokemon base experience should be greater than (.*)")]
    public async Task ThenThePokemonBaseExperienceShouldBeGreaterThan(int minValue)
    {
        Assert.That(_lastPokemon, Is.Not.Null, "No pokemon data received");
        Assert.That(_lastPokemon!.Base_Experience, Is.GreaterThan(minValue),
            $"Expected base experience > {minValue} but got {_lastPokemon.Base_Experience}");
        
        await Task.CompletedTask;
    }
}
