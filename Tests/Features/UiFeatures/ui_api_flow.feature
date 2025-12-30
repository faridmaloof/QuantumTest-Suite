@allure.parentSuite:E2E-Tests
@allure.suite:Hybrid-Flows
@allure.feature:UI-API-Integration
@allure.owner:QA-Team
Feature: UI + API Flow
  Crear usuario via API y validarlo en UI de búsqueda

  @ui @api @e2e
  @allure.story:GitHub-User-Verification
  @allure.severity:normal
  Scenario: Crear usuario y validar login
    Given un usuario de GitHub llamado 'octocat' existe via API
    When busca el usuario en UI
    Then el resultado muestra el usuario
