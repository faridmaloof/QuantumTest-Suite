using Microsoft.Playwright;
using QuantumTestSuite.API.Helpers;
using QuantumTestSuite.API.Models;

namespace QuantumTestSuite.API.Clients;

public class GitHubUserClient
{
    private readonly IAPIRequestContext _context;

    public GitHubUserClient(IAPIRequestContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<GitHubSearchResponse>> SearchAsync(string user)
    {
        var response = await _context.GetAsync($"/search/users?q={user}");
        return await ApiResponseMapper.FromAsync<GitHubSearchResponse>(response);
    }
}
