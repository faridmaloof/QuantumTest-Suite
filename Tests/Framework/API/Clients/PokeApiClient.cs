using System.Net.Http;
using System.Text.Json;
using QuantumTestSuite.Framework.API.Models;

namespace QuantumTestSuite.Framework.API.Clients;

/// <summary>
/// Client for interacting with PokeAPI
/// https://pokeapi.co/
/// </summary>
public class PokeApiClient(HttpClient? httpClient = null)
{
    private readonly HttpClient _httpClient = httpClient ?? new HttpClient { BaseAddress = new Uri("https://pokeapi.co/api/v2/") };
    private readonly JsonSerializerOptions _jsonOptions= new() { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// Get a Pokemon by ID
    /// </summary>
    public async Task<(Pokemon? Data, HttpResponseMessage Response)> GetPokemonByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"pokemon/{id}");
        
        if (!response.IsSuccessStatusCode)
            return (null, response);

        var content = await response.Content.ReadAsStringAsync();
        var pokemon = JsonSerializer.Deserialize<Pokemon>(content, _jsonOptions);
        
        return (pokemon, response);
    }

    /// <summary>
    /// Get a Pokemon by name
    /// </summary>
    public async Task<(Pokemon? Data, HttpResponseMessage Response)> GetPokemonByNameAsync(string name)
    {
        var response = await _httpClient.GetAsync($"pokemon/{name.ToLower()}");
        
        if (!response.IsSuccessStatusCode)
            return (null, response);

        var content = await response.Content.ReadAsStringAsync();
        var pokemon = JsonSerializer.Deserialize<Pokemon>(content, _jsonOptions);
        
        return (pokemon, response);
    }

    /// <summary>
    /// Get a list of Pokemon with pagination
    /// </summary>
    public async Task<(PokemonList? Data, HttpResponseMessage Response)> GetPokemonListAsync(int limit = 20, int offset = 0)
    {
        var response = await _httpClient.GetAsync($"pokemon?limit={limit}&offset={offset}");
        
        if (!response.IsSuccessStatusCode)
            return (null, response);

        var content = await response.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<PokemonList>(content, _jsonOptions);
        
        return (list, response);
    }
}

public class PokemonList
{
    public int Count { get; set; }
    public string? Next { get; set; }
    public string? Previous { get; set; }
    public List<NamedResource> Results { get; set; } = new();
}
