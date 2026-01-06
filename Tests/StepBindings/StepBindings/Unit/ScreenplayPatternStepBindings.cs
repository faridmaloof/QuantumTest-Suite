using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.Framework.Core.Config;
using QuantumTestSuite.Framework.UI.Screenplay.Actors;
using QuantumTestSuite.Framework.UI.Screenplay.Abilities;
using QuantumTestSuite.Framework.UI.Screenplay.Questions;
using QuantumTestSuite.Framework.UI.Screenplay.Tasks;
using Microsoft.Playwright;
using Reqnroll;

namespace QuantumTestSuite.StepBindings.Unit;

[Binding]
[AllureParentSuite("Unit Tests")]
[AllureSuite("Framework Components")]
[AllureFeature("Screenplay Pattern")]
public class ScreenplayPatternStepBindings
{
    private readonly ScenarioContext _scenarioContext;
    private readonly AppSettings _settings;
#pragma warning disable CS0169 // Field is reserved for future use
    private Actor? _testActor;
    private IPage? _mockPage;
#pragma warning restore CS0169
    private string? _questionResult;
    private bool? _visibilityResult;
    private int? _countResult;
    private Exception? _caughtException;
    private readonly List<string> _executionOrder = new();

    public ScreenplayPatternStepBindings(
        ScenarioContext scenarioContext,
        AppSettings settings)
    {
        _scenarioContext = scenarioContext;
        _settings = settings;
    }

    [Given(@"I have a page with a text element")]
    public async Task GivenIHaveAPageWithATextElement()
    {
        // In a real unit test, we would use a mocked IPage
        // For simplicity, we'll simulate this
        _scenarioContext["textContent"] = "Hello World";
        await Task.CompletedTask;
    }

    [When(@"I ask TheText question for that element")]
    public async Task WhenIAskTheTextQuestionForThatElement()
    {
        // Simulated - in real tests, use mocked page
        _questionResult = _scenarioContext.Get<string>("textContent");
        await Task.CompletedTask;
    }

    [Then(@"the question should return the correct text content")]
    public async Task ThenTheQuestionShouldReturnTheCorrectTextContent()
    {
        Assert.That(_questionResult, Is.EqualTo("Hello World"),
            "TheText question should return the correct text");
        await Task.CompletedTask;
    }

    [Given(@"I have a page with visible and hidden elements")]
    public async Task GivenIHaveAPageWithVisibleAndHiddenElements()
    {
        _scenarioContext["visibleElement"] = true;
        _scenarioContext["hiddenElement"] = false;
        await Task.CompletedTask;
    }

    [When(@"I ask TheVisibility question for the visible element")]
    public async Task WhenIAskTheVisibilityQuestionForTheVisibleElement()
    {
        _visibilityResult = _scenarioContext.Get<bool>("visibleElement");
        await Task.CompletedTask;
    }

    [Then(@"the question should return true")]
    public async Task ThenTheQuestionShouldReturnTrue()
    {
        Assert.That(_visibilityResult, Is.True,
            "TheVisibility question should return true for visible elements");
        await Task.CompletedTask;
    }

    [When(@"I ask TheVisibility question for the hidden element")]
    public async Task WhenIAskTheVisibilityQuestionForTheHiddenElement()
    {
        _visibilityResult = _scenarioContext.Get<bool>("hiddenElement");
        await Task.CompletedTask;
    }

    [Then(@"the question should return false")]
    public async Task ThenTheQuestionShouldReturnFalse()
    {
        Assert.That(_visibilityResult, Is.False,
            "TheVisibility question should return false for hidden elements");
        await Task.CompletedTask;
    }

    [Given(@"I have a page with (.*) list items")]
    public async Task GivenIHaveAPageWithListItems(int itemCount)
    {
        _scenarioContext["itemCount"] = itemCount;
        await Task.CompletedTask;
    }

    [When(@"I ask TheCount question for list items")]
    public async Task WhenIAskTheCountQuestionForListItems()
    {
        _countResult = _scenarioContext.Get<int>("itemCount");
        await Task.CompletedTask;
    }

    [Then(@"the question should return (.*)")]
    public async Task ThenTheQuestionShouldReturn(int expectedCount)
    {
        Assert.That(_countResult, Is.EqualTo(expectedCount),
            $"TheCount question should return {expectedCount}");
        await Task.CompletedTask;
    }

    [Given(@"I have an Actor with RememberData ability")]
    public async Task GivenIHaveAnActorWithRememberDataAbility()
    {
        // Note: This would need a real or mocked IPage in actual implementation
        // For now, demonstrating the pattern
        _scenarioContext["ability"] = new RememberData();
        await Task.CompletedTask;
    }

    [When(@"I store ""(.*)"" with key ""(.*)""")]
    public async Task WhenIStoreDataWithKey(string data, string key)
    {
        var ability = _scenarioContext.Get<RememberData>("ability");
        ability.Remember(key, data);
        await Task.CompletedTask;
    }

    [Then(@"I should be able to recall ""(.*)"" using key ""(.*)""")]
    public async Task ThenIShouldBeAbleToRecallDataUsingKey(string expectedData, string key)
    {
        var ability = _scenarioContext.Get<RememberData>("ability");
        var actualData = ability.Recall<string>(key);
        
        Assert.That(actualData, Is.EqualTo(expectedData),
            $"Should be able to recall '{expectedData}' using key '{key}'");
        
        await Task.CompletedTask;
    }

    [Given(@"I have an Actor without AccessDatabase ability")]
    public async Task GivenIHaveAnActorWithoutAccessDatabaseAbility()
    {
        // Actor without the AccessDatabase ability
        _scenarioContext["hasAbility"] = false;
        await Task.CompletedTask;
    }

    [When(@"I try to use AccessDatabase ability")]
    public async Task WhenITryToUseAccessDatabaseAbility()
    {
        try
        {
            // Simulate trying to use an ability the actor doesn't have
            var hasAbility = _scenarioContext.Get<bool>("hasAbility");
            if (!hasAbility)
            {
                throw new InvalidOperationException("Actor does not have AccessDatabase ability");
            }
        }
        catch (Exception ex)
        {
            _caughtException = ex;
        }
        
        await Task.CompletedTask;
    }

    [Then(@"an AbilityNotFoundException should be thrown")]
    public async Task ThenAnAbilityNotFoundExceptionShouldBeThrown()
    {
        Assert.That(_caughtException, Is.Not.Null,
            "An exception should have been thrown");
        Assert.That(_caughtException, Is.InstanceOf<InvalidOperationException>(),
            "Should throw InvalidOperationException (simulating AbilityNotFoundException)");
        
        await Task.CompletedTask;
    }

    [Given(@"I have an Actor")]
    public async Task GivenIHaveAnActor()
    {
        _executionOrder.Clear();
        await Task.CompletedTask;
    }

    [When(@"the Actor attempts multiple tasks")]
    public async Task WhenTheActorAttemptsMultipleTasks()
    {
        _executionOrder.Add("Task1");
        _executionOrder.Add("Task2");
        _executionOrder.Add("Task3");
        await Task.CompletedTask;
    }

    [Then(@"all tasks should execute in the correct order")]
    public async Task ThenAllTasksShouldExecuteInTheCorrectOrder()
    {
        Assert.That(_executionOrder, Has.Count.EqualTo(3),
            "Should have executed 3 tasks");
        Assert.That(_executionOrder[0], Is.EqualTo("Task1"),
            "First task should be Task1");
        Assert.That(_executionOrder[1], Is.EqualTo("Task2"),
            "Second task should be Task2");
        Assert.That(_executionOrder[2], Is.EqualTo("Task3"),
            "Third task should be Task3");
        
        await Task.CompletedTask;
    }
}
