@unit @smoke
@allure.parentSuite:Unit-Tests
@allure.suite:Configuration
@allure.feature:Config-Manager
@allure.owner:QA-Team
Feature: Configuration Manager Unit Tests
  Como desarrollador del framework
  Quiero validar que ConfigManager funciona correctamente
  Para asegurar que la configuración se carga en el orden correcto

  Background:
    Given el sistema tiene ConfigManager inicializado

  @priority-high
  Scenario: Configuración carga valores por defecto cuando no hay overrides
    When no se establecen variables de entorno
    And no existe archivo .env
    Then ConfigManager debe retornar valores por defecto
    And Playwright.Headless debe ser "true"
    And Playwright.Browser debe ser "chromium"

  @priority-high @ignore
  Scenario: Variables de entorno tienen mayor prioridad que .env
    Given existe un archivo .env con "PLAYWRIGHT_HEADLESS=true"
    When se establece variable de entorno "PLAYWRIGHT_HEADLESS=false"
    Then ConfigManager.Settings.Playwright.Headless debe ser "false"

  @priority-medium @ignore
  Scenario: User Secrets tienen prioridad sobre .env
    Given User Secrets contiene "Playwright:Browser=firefox"
    And existe un archivo .env con "PLAYWRIGHT_BROWSER=chromium"
    And no hay variables de entorno establecidas
    Then ConfigManager.Settings.Playwright.Browser debe ser "firefox"

  @priority-medium @ignore
  Scenario: Archivo .env específico de entorno sobrescribe .env base
    Given existe un archivo .env con "PLAYWRIGHT_BASE_URL=https://base.com"
    And existe un archivo .env.qa con "PLAYWRIGHT_BASE_URL=https://qa.com"
    When se establece variable de entorno "TEST_ENVIRONMENT=qa"
    Then ConfigManager.Settings.Playwright.BaseUrl debe contener "qa.com"

  @priority-high
  Scenario Outline: Validar parsing de variables booleanas
    When se establece variable de entorno "<variable>=<valor>"
    Then ConfigManager debe parsear como booleano "<esperado>"

    Examples:
      | variable                 | valor | esperado |
      | PLAYWRIGHT_HEADLESS      | true  | true     |
      | PLAYWRIGHT_HEADLESS      | false | false    |
      | PLAYWRIGHT_VIDEO_ENABLED | True  | true     |
      | PLAYWRIGHT_VIDEO_ENABLED | FALSE | false    |

  @priority-medium
  Scenario: ConfigManager es singleton y no recarga en cada acceso
    Given ConfigManager.Settings se accede por primera vez
    When se modifica una variable de entorno después de la primera carga
    Then ConfigManager.Settings debe retornar la configuración cacheada
    And no debe recargar desde archivos
