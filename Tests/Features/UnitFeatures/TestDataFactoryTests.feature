@unit @smoke
@allure.parentSuite:Unit-Tests
@allure.suite:Test-Data
@allure.feature:Data-Factories
@allure.owner:Framework-Team
Feature: Test Data Factory Unit Tests
  Como desarrollador de tests
  Quiero validar que las factories generan datos correctos
  Para asegurar datos de prueba confiables

  Background:
    Given las factories están inicializadas

  @priority-high
  @allure.story:BookingFactory
  @allure.severity:critical
  Scenario: BookingFactory genera datos válidos
    When se genera un booking usando BookingFactory
    Then el booking debe tener firstname no vacío
    And el booking debe tener lastname no vacío
    And el booking debe tener totalprice mayor a 0
    And el booking debe tener depositpaid como booleano válido
    And bookingdates debe tener checkin y checkout válidos
    And checkin debe ser anterior a checkout

  @priority-medium
  Scenario: BookingFactory genera datos únicos en múltiples llamadas
    When se generan 5 bookings usando BookingFactory
    Then todos los bookings deben tener diferentes firstnames
    And todos los bookings deben tener diferentes lastnames

  @priority-medium
  Scenario: Factory respeta valores personalizados
    When se genera un booking con firstname "John" usando BookingFactory
    Then el booking debe tener firstname "John"
    And los demás campos deben ser generados automáticamente

  @priority-low @ignore
  Scenario: Factory genera datos dentro de rangos esperados
    When se generan 10 bookings usando BookingFactory
    Then todos los precios deben estar entre 50 y 2000
    And todas las fechas de checkout deben ser después de checkin
    And todas las fechas deben ser en el futuro
