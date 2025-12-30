using Microsoft.Playwright;
using QuantumTestSuite.API.Helpers;
using QuantumTestSuite.API.Models;

namespace QuantumTestSuite.Core.Services;

public interface IApiService
{
    Task<IAPIRequestContext> CreateContextAsync(string baseUrl);
}

public interface IBookingService
{
    Task<ApiResponse<BookingResponse>> CreateBookingAsync(BookingRequest request);
}

public interface IGitHubService
{
    Task<ApiResponse<GitHubSearchResponse>> SearchUserAsync(string username);
}

public interface IHttpBinService
{
    Task<ApiResponse<HttpBinGetResponse>> GetAsync();
}
