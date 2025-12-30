@allure.parentSuite:UI-Tests
@allure.suite:Forms
@allure.feature:Contact-Form
@allure.owner:UI-Team
Feature: Ultimate QA form
  Completar y enviar el formulario público

  @ui @regression
  @allure.story:Submit-Contact-Form
  @allure.severity:normal
  Scenario: Enviar formulario de contacto
    Given el usuario abre el formulario de Ultimate QA
    When completa el formulario de contacto
    Then visualiza confirmación de envío
