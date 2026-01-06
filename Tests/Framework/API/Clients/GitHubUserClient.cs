using Microsoft.Playwright;
using QuantumTestSuite.Framework.API.Helpers;
using QuantumTestSuite.Framework.API.Models;

namespace QuantumTestSuite.Framework.API.Clients;

public class GitHubUserClient(IAPIRequestContext context)
{
    private readonly IAPIRequestContext _context = context;

    public async Task<ApiResponse<GitHubSearchResponse>> SearchAsync(string user)
    {
        var response = await _context.GetAsync($"/search/users?q={user}");
        return await ApiResponseMapper.FromAsync<GitHubSearchResponse>(response);
    }
}
