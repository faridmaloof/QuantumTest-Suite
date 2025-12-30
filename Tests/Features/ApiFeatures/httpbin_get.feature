@allure.parentSuite:API-Tests
@allure.suite:HTTPBin
@allure.feature:GET-Requests
@allure.owner:API-Team
Feature: HTTPBin GET
  Como QA quiero validar el endpoint público de HTTPBin

  @api @smoke
  @allure.story:Basic-GET-Request
  @allure.severity:critical
  Scenario: GET /get returns 200
    Given una llamada a GET /get
    When se ejecuta la petición
    Then el status debe ser 200
