@unit @utilities
@allure.parentSuite:Unit-Tests
@allure.suite:Utilities
@allure.feature:Helper-Functions
@allure.owner:Framework-Team
Feature: Utilities Helper Functions Unit Tests
  Como desarrollador
  Quiero validar que las funciones helper funcionan correctamente
  Para asegurar código reutilizable confiable

  @retry @priority-high
  @allure.story:RetryHelper
  @allure.severity:critical
  Scenario: RetryHelper ejecuta acción exitosa sin reintentos
    Given una acción que siempre tiene éxito
    When se ejecuta con RetryHelper con maxAttempts=3
    Then la acción debe ejecutarse exactamente 1 vez
    And debe retornar resultado exitoso

  @retry @priority-high
  Scenario: RetryHelper reintenta hasta maxAttempts en caso de fallo
    Given una acción que siempre falla
    When se ejecuta con RetryHelper con maxAttempts=3
    Then la acción debe ejecutarse exactamente 3 veces
    And debe lanzar excepción después del último intento

  @retry @priority-medium
  Scenario: RetryHelper tiene éxito después de reintentos
    Given una acción que falla 2 veces y luego tiene éxito
    When se ejecuta con RetryHelper con maxAttempts=5
    Then la acción debe ejecutarse exactamente 3 veces
    And debe retornar resultado exitoso

  @timeouts @priority-high
  Scenario: TestTimeouts provee constantes correctas
    Then TestTimeouts.Short debe ser menor a TestTimeouts.Default
    And TestTimeouts.Default debe ser menor a TestTimeouts.Long
    And TestTimeouts.Extended debe ser el mayor timeout

  @wait @priority-medium
  Scenario: WaitHelper espera por condición con éxito
    Given una condición que se cumple después de 2 segundos
    When se usa WaitHelper con timeout de 5 segundos
    Then debe retornar true cuando la condición se cumpla
    And debe completar antes del timeout

  @wait @priority-medium
  Scenario: WaitHelper timeout cuando condición no se cumple
    Given una condición que nunca se cumple
    When se usa WaitHelper con timeout de 2 segundos
    Then debe lanzar TimeoutException después de 2 segundos
