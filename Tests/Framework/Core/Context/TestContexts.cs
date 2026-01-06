using Microsoft.Playwright;
using QuantumTestSuite.Framework.API.Helpers;
using QuantumTestSuite.Framework.API.Models;

namespace QuantumTestSuite.Framework.Core.Context;

public abstract class BaseTestContext
{
    public IPlaywright? Playwright { get; set; }
    public IBrowser? Browser { get; set; }
    public IBrowserContext? BrowserContext { get; set; }
    public IPage? Page { get; set; }
}

public class ApiTestContext : BaseTestContext
{
    public IAPIRequestContext? ApiContext { get; set; }
    public string? LastResponse { get; set; }
    public ApiResponse<BookingResponse>? BookingResponse { get; set; }
    public ApiResponse<HttpBinGetResponse>? HttpBinResponse { get; set; }
    public ApiResponse<GitHubSearchResponse>? GitHubResponse { get; set; }
    public string? GitHubUser { get; set; }
    public BookingRequest? BookingPayload { get; set; }
}

public class UiTestContext : BaseTestContext
{
    public string? CurrentUrl { get; set; }
    public string? ActorName { get; set; }
}
