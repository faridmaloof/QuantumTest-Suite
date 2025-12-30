@allure.parentSuite:UI-Tests
@allure.suite:Authentication
@allure.feature:Login
@allure.owner:UI-Team
Feature: SauceDemo Login
  Validar login exitoso en SauceDemo

  @ui @smoke
  @allure.story:Valid-Credentials-Login
  @allure.severity:critical
  Scenario: Login exitoso
    Given el usuario está en la página de SauceDemo
    When ingresa credenciales válidas
    Then debe ver la página de inventario
