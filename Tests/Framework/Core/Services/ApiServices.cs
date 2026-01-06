using Microsoft.Playwright;
using QuantumTestSuite.Framework.API.Clients;
using QuantumTestSuite.Framework.API.Helpers;
using QuantumTestSuite.Framework.API.Models;
using QuantumTestSuite.Framework.Core.Config;

namespace QuantumTestSuite.Framework.Core.Services;

public class BookingService : IBookingService
{
    private readonly IPlaywright _playwright;
    private readonly AppSettings _settings;
    private IAPIRequestContext? _context;

    public BookingService(IPlaywright playwright, AppSettings settings)
    {
        _playwright = playwright;
        _settings = settings;
    }

    public async Task<ApiResponse<BookingResponse>> CreateBookingAsync(BookingRequest request)
    {
        _context ??= await ApiRequestContextFactory.CreateAsync(_playwright, _settings.Apis.RestfulBooker);
        var client = new RestfulBookerClient(_context);
        return await client.CreateBookingAsync(request);
    }
}

public class GitHubService : IGitHubService
{
    private readonly IPlaywright _playwright;
    private readonly AppSettings _settings;
    private IAPIRequestContext? _context;

    public GitHubService(IPlaywright playwright, AppSettings settings)
    {
        _playwright = playwright;
        _settings = settings;
    }

    public async Task<ApiResponse<GitHubSearchResponse>> SearchUserAsync(string username)
    {
        _context ??= await ApiRequestContextFactory.CreateAsync(_playwright, _settings.Apis.GitHub);
        var client = new GitHubUserClient(_context);
        return await client.SearchAsync(username);
    }
}

public class HttpBinService : IHttpBinService
{
    private readonly IPlaywright _playwright;
    private readonly AppSettings _settings;
    private IAPIRequestContext? _context;

    public HttpBinService(IPlaywright playwright, AppSettings settings)
    {
        _playwright = playwright;
        _settings = settings;
    }

    public async Task<ApiResponse<HttpBinGetResponse>> GetAsync()
    {
        _context ??= await ApiRequestContextFactory.CreateAsync(_playwright, _settings.Apis.HttpBin);
        var client = new HttpBinClient(_context);
        return await client.GetAsync();
    }
}
