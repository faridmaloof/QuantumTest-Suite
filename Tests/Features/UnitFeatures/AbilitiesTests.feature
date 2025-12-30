@unit @abilities
@allure.parentSuite:Unit-Tests
@allure.suite:Screenplay-Pattern
@allure.feature:Abilities-System
@allure.owner:Framework-Team
Feature: Abilities System Unit Tests
  Como desarrollador del framework
  Quiero validar que el sistema de Abilities funciona correctamente
  Para asegurar la robustez del patrón Screenplay

  Background:
    Given tengo un Actor con una página mock

  @abilities-basic @ignore
  @allure.story:Ability-Management
  @allure.severity:critical
  Scenario: Actor puede obtener Abilities que tiene
    Given el Actor tiene la ability RememberData
    When intento usar la ability RememberData
    Then la ability debe estar disponible

  @abilities-exception @ignore
  Scenario: Actor lanza excepción al usar Ability que no tiene
    Given el Actor NO tiene la ability AccessDatabase
    When intento usar la ability AccessDatabase
    Then debe lanzar AbilityNotFoundException

  @abilities-fluent @ignore
  Scenario: WhoCan permite asignar múltiples Abilities de forma fluida
    When asigno múltiples Abilities al Actor con WhoCan
    Then el Actor debe tener todas las Abilities asignadas

  @remember-data @ignore
  Scenario: RememberData puede almacenar y recuperar datos
    Given el Actor tiene la ability RememberData
    When almaceno un valor con clave "testKey"
    Then puedo recuperar el valor usando la misma clave

  @remember-data-typed @ignore
  Scenario: RememberData mantiene type-safety
    Given el Actor tiene la ability RememberData
    When almaceno un objeto User con clave "user"
    Then puedo recuperar el User usando la misma clave
    And debe lanzar InvalidCastException si intento recuperarlo como string

  @remember-data-forget @ignore
  Scenario: RememberData puede olvidar datos almacenados
    Given el Actor tiene la ability RememberData
    And almaceno un valor con clave "temp"
    When olvido la clave "temp"
    Then la clave "temp" no debe existir en memoria

  @read-configuration @ignore
  Scenario: ReadConfiguration provee acceso a AppSettings
    Given el Actor tiene la ability ReadConfiguration
    When solicito la URL base
    Then debo obtener la URL configurada

  @abilities-cleanup @ignore
  Scenario: Actor ejecuta cleanup en todas las Abilities
    Given el Actor tiene múltiples Abilities
    When ejecuto CleanupAsync en el Actor
    Then todas las Abilities deben limpiar sus recursos
