namespace QuantumTestSuite.UI.Screenplay.Abilities;

/// <summary>
/// Exception thrown when an Actor attempts to use an Ability they don't have.
/// </summary>
public class AbilityNotFoundException(string actorName, Type abilityType) 
    : InvalidOperationException($"Actor '{actorName}' does not have the ability '{abilityType.Name}'. " +
               $"Use actor.WhoCan(new {abilityType.Name}()) to grant this ability.")
{
    public string ActorName { get; } = actorName;
    public Type AbilityType { get; } = abilityType;
}
