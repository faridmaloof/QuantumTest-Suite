@allure.parentSuite:UI-Tests
@allure.suite:Playwright-Demo
@allure.feature:TodoMVC
@allure.owner:QA-Team
Feature: Playwright TodoMVC Demo
  As a QA Engineer
  I want to test the TodoMVC application
  So that I can validate my test automation framework with Questions pattern

  @ui @smoke @demo
  @allure.story:TodoManagement
  @allure.severity:critical
  Scenario: Add and verify items in todo list
    Given the user navigates to the Playwright demo page
    When the user adds "Buy groceries" to the list
    And the user adds "Walk the dog" to the list
    And the user adds "Read a book" to the list
    Then the list should contain 3 items
    And the list should include "Buy groceries"
    And the list should include "Walk the dog"
    And the list should include "Read a book"

  @ui @regression @demo
  @allure.story:TodoManagement
  @allure.severity:high
  Scenario: Verify remaining items counter
    Given the user navigates to the Playwright demo page
    When the user adds "Task 1" to the list
    And the user adds "Task 2" to the list
    Then the list should contain 2 items
    And the remaining count should show "2 items left"

  @ui @regression @demo
  @allure.story:TodoManagement
  @allure.severity:medium
  Scenario: Complete todo items
    Given the user navigates to the Playwright demo page
    When the user adds "Complete this task" to the list
    And the user adds "Another task" to the list
    And the user toggles todo item 1
    Then todo item 1 should be completed
    And the list should contain 2 items

