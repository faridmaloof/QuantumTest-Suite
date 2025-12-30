using Microsoft.Playwright;
using QuantumTestSuite.API.Helpers;
using QuantumTestSuite.API.Models;

namespace QuantumTestSuite.API.Clients;

public class HttpBinClient
{
    private readonly IAPIRequestContext _context;

    public HttpBinClient(IAPIRequestContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<HttpBinGetResponse>> GetAsync(string path = "/get")
    {
        var response = await _context.GetAsync(path);
        return await ApiResponseMapper.FromAsync<HttpBinGetResponse>(response);
    }
}
