namespace QuantumTestSuite.UI.Screenplay.Abilities;

/// <summary>
/// Exception thrown when an Actor attempts to use an Ability they don't have.
/// </summary>
public class AbilityNotFoundException : InvalidOperationException
{
    public AbilityNotFoundException(string actorName, Type abilityType)
        : base($"Actor '{actorName}' does not have the ability '{abilityType.Name}'. " +
               $"Use actor.WhoCan(new {abilityType.Name}()) to grant this ability.")
    {
        ActorName = actorName;
        AbilityType = abilityType;
    }

    public string ActorName { get; }
    public Type AbilityType { get; }
}
