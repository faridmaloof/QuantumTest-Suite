@allure.parentSuite:API-Tests
@allure.suite:Pokemon-API
@allure.feature:PokemonData
@allure.owner:QA-Team
Feature: Pokemon API Testing
  As a QA Engineer
  I want to test the PokeAPI endpoints
  So that I can validate API responses and data integrity

  @api @smoke
  @allure.story:GetPokemon
  @allure.severity:critical
  Scenario: Retrieve Pokemon by ID
    When I request pokemon with ID 25
    Then the response status should be 200
    And the pokemon name should be "pikachu"
    And the pokemon should have "electric" type

  @api @smoke
  @allure.story:GetPokemon
  @allure.severity:critical
  Scenario: Retrieve Pokemon by name
    When I request pokemon "charizard"
    Then the response status should be 200
    And the pokemon ID should be 6
    And the pokemon should have 2 types

  @api @regression
  @allure.story:PokemonAbilities
  @allure.severity:high
  Scenario: Validate Pokemon abilities
    When I request pokemon "bulbasaur"
    Then the response status should be 200
    And the pokemon should have the ability "overgrow"
    And the pokemon should have at least 1 ability

  @api @regression
  @allure.story:PokemonStats
  @allure.severity:medium
  Scenario: Verify Pokemon stats
    When I request pokemon with ID 1
    Then the response status should be 200
    And the pokemon should have 6 stats
    And the pokemon base experience should be greater than 0

  @api @negative
  @allure.story:ErrorHandling
  @allure.severity:high
  Scenario: Handle non-existent Pokemon
    When I request pokemon with ID 99999
    Then the response status should be 404
