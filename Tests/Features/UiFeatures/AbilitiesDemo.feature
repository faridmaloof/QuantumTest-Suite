@ui @abilities @smoke
Feature: Login with Database Users using Abilities
  Como tester del framework
  Quiero usar Abilities para obtener datos de BD
  Para demostrar el patrón Screenplay con Abilities

  Background:
    Given el Actor tiene las habilidades necesarias

  @database @ignore
  Scenario: Login con usuario obtenido desde BD
    Given obtengo usuarios válidos desde la base de datos usando AccessDatabase
    When inicio sesión con el usuario recordado usando RememberData
    Then debo ver la página de inventario

  @api-hybrid @ignore
  Scenario: Crear booking mediante API y validar en UI
    Given creo un booking mediante CallApiEndpoint ability
    When busco el booking creado en la UI
    Then el booking debe estar visible con los datos correctos

  @configuration @ignore
  Scenario: Usar configuración dinámica con ReadConfiguration
    Given el Actor lee la configuración usando ReadConfiguration ability
    When navego a la URL base configurada
    Then debo estar en la página correcta según el entorno
