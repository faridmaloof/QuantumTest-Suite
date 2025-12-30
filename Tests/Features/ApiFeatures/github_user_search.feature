@allure.parentSuite:API-Tests
@allure.suite:GitHub-API
@allure.feature:User-Search
@allure.owner:API-Team
Feature: GitHub user search API
  Obtener un usuario mediante la API de GitHub para pruebas combinadas UI + API

  @api @smoke
  @allure.story:Search-User-by-Username
  @allure.severity:critical
  Scenario: Buscar octocat via API
    Given un usuario de GitHub llamado 'octocat' existe via API
    Then el status debe ser 200

  @api @smoke @lang-en
  Scenario: Search octocat via API (EN)
    Given a GitHub user named 'octocat' exists via API
    Then the status must be 200

  @api @smoke @lang-pt
  Scenario: Buscar octocat via API (PT)
    Given um usuario do GitHub chamado 'octocat' existe via API
    Then o status deve ser 200