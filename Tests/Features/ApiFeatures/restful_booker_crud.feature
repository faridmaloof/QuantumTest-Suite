@allure.parentSuite:API-Tests
@allure.suite:Restful-Booker
@allure.feature:CRUD-Operations
@allure.owner:API-Team
Feature: Restful Booker CRUD
  Validar creación de bookings en la API pública

  @api @regression
  @allure.story:Create-Booking
  @allure.severity:critical
  Scenario: Create booking and validate
    Given un payload válido de booking
    When se envía POST /booking
    Then el status debe ser 200
    And el response contiene bookingid
