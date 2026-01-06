using Microsoft.Playwright;
using QuantumTestSuite.UI.Screenplay.Abilities;

namespace QuantumTestSuite.UI.Screenplay.Actors;

/// <summary>
/// Represents an Actor in the Screenplay Pattern.
/// An Actor can perform Tasks through Interactions and has Abilities that enable those actions.
/// </summary>
public class Actor
{
    private readonly Dictionary<Type, IAbility> _abilities = new();

    public Actor(string name, IPage page)
    {
        Name = name;
        Page = page;
    }

    /// <summary>
    /// The name of this Actor (e.g., "TestUser", "Admin", "Customer")
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The Playwright page instance for UI interactions
    /// </summary>
    public IPage Page { get; }

    /// <summary>
    /// Grant an Ability to this Actor using fluent API.
    /// </summary>
    /// <param name="ability">The ability to grant</param>
    /// <returns>This Actor instance for method chaining</returns>
    /// <example>
    /// var actor = new Actor("TestUser", page)
    ///     .WhoCan(new RememberData())
    ///     .WhoCan(new AccessDatabase(connection));
    /// </example>
    public Actor WhoCan(IAbility ability)
    {
        ArgumentNullException.ThrowIfNull(ability);
        
        var abilityType = ability.GetType();
        _abilities[abilityType] = ability;
        
        // Initialize ability asynchronously (fire and forget for fluent API)
        _ = ability.InitializeAsync();
        
        return this;
    }

    /// <summary>
    /// Use an Ability that this Actor has been granted.
    /// </summary>
    /// <typeparam name="T">The type of Ability to use</typeparam>
    /// <returns>The Ability instance</returns>
    /// <exception cref="AbilityNotFoundException">Thrown when the Actor doesn't have the requested Ability</exception>
    /// <example>
    /// var users = await actor.Using&lt;AccessDatabase&gt;().GetUsersAsync();
    /// actor.Using&lt;RememberData&gt;().Remember("user", user);
    /// </example>
    public T Using<T>() where T : IAbility
    {
        var abilityType = typeof(T);
        
        if (!_abilities.TryGetValue(abilityType, out var ability))
        {
            throw new AbilityNotFoundException(Name, abilityType);
        }

        return (T)ability;
    }

    /// <summary>
    /// Check if this Actor has a specific Ability.
    /// </summary>
    /// <typeparam name="T">The type of Ability to check</typeparam>
    /// <returns>True if the Actor has the Ability, false otherwise</returns>
    public bool Has<T>() where T : IAbility
    {
        return _abilities.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Attempt to use an Ability if the Actor has it.
    /// </summary>
    /// <typeparam name="T">The type of Ability to try</typeparam>
    /// <param name="ability">The Ability instance if found</param>
    /// <returns>True if the Ability was found, false otherwise</returns>
    public bool TryUsing<T>(out T? ability) where T : IAbility
    {
        if (_abilities.TryGetValue(typeof(T), out var foundAbility))
        {
            ability = (T)foundAbility;
            return true;
        }

        ability = default;
        return false;
    }

    /// <summary>
    /// Execute one or more Tasks in sequence.
    /// </summary>
    /// <param name="tasks">The tasks to execute</param>
    public async Task AttemptsTo(params ITask[] tasks)
    {
        foreach (var task in tasks)
        {
            await task.ExecuteAsync(this);
        }
    }

    /// <summary>
    /// Ask a Question and get the answer.
    /// Questions are used to retrieve information from the system under test.
    /// </summary>
    /// <typeparam name="T">The type of answer expected</typeparam>
    /// <param name="question">The question to ask</param>
    /// <returns>The answer to the question</returns>
    /// <example>
    /// var title = await actor.Asks(TheTitle.OfThePage);
    /// var text = await actor.Asks(TheText.Of(".message"));
    /// var isVisible = await actor.Asks(TheVisibility.Of("#button"));
    /// </example>
    public async Task<T> Asks<T>(Questions.IQuestion<T> question)
    {
        return await question.AnsweredBy(this);
    }

    /// <summary>
    /// Clean up all Abilities when the Actor is no longer needed.
    /// </summary>
    public async Task CleanupAsync()
    {
        foreach (var ability in _abilities.Values)
        {
            await ability.CleanupAsync();
        }
        
        _abilities.Clear();
    }
}

public interface ITask
{
    Task ExecuteAsync(Actor actor);
}

public interface IInteraction
{
    Task PerformAsync(Actor actor);
}
