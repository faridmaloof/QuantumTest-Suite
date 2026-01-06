namespace QuantumTestSuite.Framework.UI.Screenplay.Abilities;

/// <summary>
/// Marker interface for all Actor abilities.
/// An Ability represents something that an Actor can do or has access to,
/// such as calling APIs, accessing a database, or remembering information.
/// </summary>
/// <remarks>
/// This follows the Screenplay Pattern where Actors have Abilities to perform Tasks through Interactions.
/// Example: An Actor who can RememberData, AccessDatabase, or CallApiEndpoint.
/// </remarks>
public interface IAbility
{
    /// <summary>
    /// Optional initialization logic when the ability is assigned to an Actor.
    /// </summary>
    Task InitializeAsync() 
        => Task.CompletedTask;

    /// <summary>
    /// Optional cleanup logic when the ability is no longer needed.
    /// </summary>
    Task CleanupAsync() 
        => Task.CompletedTask;
}
