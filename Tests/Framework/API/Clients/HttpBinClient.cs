using Microsoft.Playwright;
using QuantumTestSuite.Framework.API.Helpers;
using QuantumTestSuite.Framework.API.Models;

namespace QuantumTestSuite.Framework.API.Clients;

public class HttpBinClient(IAPIRequestContext context)
{
    private readonly IAPIRequestContext _context = context;

    public async Task<ApiResponse<HttpBinGetResponse>> GetAsync(string path = "/get")
    {
        var response = await _context.GetAsync(path);
        return await ApiResponseMapper.FromAsync<HttpBinGetResponse>(response);
    }
}
