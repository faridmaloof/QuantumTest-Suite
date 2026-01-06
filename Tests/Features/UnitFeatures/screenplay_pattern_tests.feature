@allure.parentSuite:Unit-Tests
@allure.suite:Framework-Components
@allure.feature:ScreenplayPattern
@allure.owner:QA-Team
Feature: Screenplay Pattern Components Unit Tests
  As a Framework Developer
  I want to test the Screenplay Pattern components
  So that I ensure the framework works correctly

  @unit @smoke
  @allure.story:Questions
  @allure.severity:critical
  Scenario: TheText Question returns correct element text
    Given I have a page with a text element
    When I ask TheText question for that element
    Then the question should return the correct text content

  @unit @smoke
  @allure.story:Questions
  @allure.severity:critical
  Scenario: TheVisibility Question checks element visibility correctly
    Given I have a page with visible and hidden elements
    When I ask TheVisibility question for the visible element
    Then the question should return true
    When I ask TheVisibility question for the hidden element
    Then the question should return false

  @unit @regression
  @allure.story:Questions
  @allure.severity:high
  Scenario: TheCount Question counts elements accurately
    Given I have a page with 5 list items
    When I ask TheCount question for list items
    Then the question should return 5

  @unit @regression
  @allure.story:Abilities
  @allure.severity:high
  Scenario: Actor can use granted Abilities
    Given I have an Actor with RememberData ability
    When I store "test data" with key "myKey"
    Then I should be able to recall "test data" using key "myKey"

  @unit @regression
  @allure.story:Abilities
  @allure.severity:medium
  Scenario: Actor throws exception for missing Abilities
    Given I have an Actor without AccessDatabase ability
    When I try to use AccessDatabase ability
    Then an AbilityNotFoundException should be thrown

  @unit @smoke
  @allure.story:Tasks
  @allure.severity:critical
  Scenario: Actor executes Tasks in sequence
    Given I have an Actor
    When the Actor attempts multiple tasks
    Then all tasks should execute in the correct order
