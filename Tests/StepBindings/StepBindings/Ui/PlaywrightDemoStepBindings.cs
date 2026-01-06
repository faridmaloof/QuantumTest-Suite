using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.Framework.Core.Config;
using QuantumTestSuite.StepBindings.Base;
using QuantumTestSuite.Framework.UI.Screenplay.Tasks;
using QuantumTestSuite.Framework.UI.Screenplay.Questions;
using QuantumTestSuite.Framework.UI.Pages;
using Reqnroll;

namespace QuantumTestSuite.StepBindings.Ui;

[Binding]
[AllureParentSuite("UI Tests")]
[AllureSuite("Playwright Demo")]
[AllureFeature("TodoMVC")]
public class PlaywrightDemoStepBindings : UiStepBindingsBase
{
    public PlaywrightDemoStepBindings(
        ScenarioContext scenarioContext,
        AppSettings settings)
        : base(scenarioContext, settings)
    {
    }

    [Given(@"the user navigates to the Playwright demo page")]
    [Given(@"the user is on the Playwright demo page")]
    public async Task GivenTheUserNavigatesToThePlaywrightDemoPage()
    {
        await ExecuteGivenAsync(async () =>
        {
            await EnsureActorAsync("DemoUser");
            await Actor!.AttemptsTo(NavigateToPlaywrightDemo.Page);
        });
    }

    [When(@"the user adds ""(.*)"" to the list")]
    public async Task WhenTheUserAddsItemToTheList(string itemText)
    {
        await ExecuteWhenAsync(async () =>
        {
            await Actor!.AttemptsTo(AddTodoItem.With(itemText));
        });
    }

    [When(@"the user toggles todo item (.*)")]
    public async Task WhenTheUserTogglesTodItem(int itemIndex)
    {
        await ExecuteWhenAsync(async () =>
        {
            var page = new PlaywrightDemoPage(Actor!.Page, Settings);
            await page.ToggleTodoAsync(itemIndex);
        });
    }

    [Then(@"the list should contain (.*) items")]
    public async Task ThenTheListShouldContainItems(int expectedCount)
    {
        await ExecuteThenAsync(async () =>
        {
            var actualCount = await Actor!.Asks(TheCount.Of(QuantumTestSuite.Framework.UI.Locators.PlaywrightDemoLocators.TodoItem));
            
            Assert.That(actualCount, Is.EqualTo(expectedCount),
                $"Expected {expectedCount} items but found {actualCount}");
        });
    }

    [Then(@"the list should include ""(.*)""")]
    public async Task ThenTheListShouldIncludeItem(string expectedItem)
    {
        await ExecuteThenAsync(async () =>
        {
            var items = await Actor!.Asks(TheTodoItems.Text);
            
            Assert.That(items, Does.Contain(expectedItem),
                $"Expected list to contain '{expectedItem}' but it didn't. Actual items: {string.Join(", ", items)}");
        });
    }

    [Then(@"the remaining count should show ""(.*)""")]
    public async Task ThenTheRemainingCountShouldShow(string expectedText)
    {
        await ExecuteThenAsync(async () =>
        {
            var page = new PlaywrightDemoPage(Actor!.Page, Settings);
            var actualText = await page.GetRemainingCountTextAsync();
            
            Assert.That(actualText, Does.Contain(expectedText),
                $"Expected remaining count to show '{expectedText}' but got '{actualText}'");
        });
    }

    [Then(@"todo item (.*) should be completed")]
    public async Task ThenTodoItemShouldBeCompleted(int itemIndex)
    {
        await ExecuteThenAsync(async () =>
        {
            var page = new PlaywrightDemoPage(Actor!.Page, Settings);
            var isCompleted = await page.IsTodoCompletedAsync(itemIndex);
            
            Assert.That(isCompleted, Is.True,
                $"Expected todo item {itemIndex} to be completed");
        });
    }
}

