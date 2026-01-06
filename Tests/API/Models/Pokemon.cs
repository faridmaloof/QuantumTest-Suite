namespace QuantumTestSuite.API.Models;

/// <summary>
/// Model representing a Pokemon from PokeAPI
/// </summary>
public class Pokemon
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Base_Experience { get; set; }
    public int Height { get; set; }
    public int Weight { get; set; }
    public List<PokemonType> Types { get; set; } = new();
    public List<PokemonAbility> Abilities { get; set; } = new();
    public List<PokemonStat> Stats { get; set; } = new();
}

public class PokemonType
{
    public int Slot { get; set; }
    public NamedResource Type { get; set; } = new();
}

public class PokemonAbility
{
    public bool Is_Hidden { get; set; }
    public int Slot { get; set; }
    public NamedResource Ability { get; set; } = new();
}

public class PokemonStat
{
    public int Base_Stat { get; set; }
    public int Effort { get; set; }
    public NamedResource Stat { get; set; } = new();
}

public class NamedResource
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
