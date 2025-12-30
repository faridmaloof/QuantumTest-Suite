using System.Text.Json;
using Microsoft.Playwright;
using QuantumTestSuite.API.Helpers;
using QuantumTestSuite.API.Models;
using QuantumTestSuite.Core.Reporting;

namespace QuantumTestSuite.API.Clients;

public class RestfulBookerClient
{
    private readonly IAPIRequestContext _context;

    public RestfulBookerClient(IAPIRequestContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<BookingResponse>> CreateBookingAsync(BookingRequest request)
    {
        var payload = JsonSerializer.Serialize(request);
        AllureHelper.AttachJson("booking-request", payload);

        var response = await _context.PostAsync("/booking", new APIRequestContextOptions
        {
            DataObject = request,
            Headers = new Dictionary<string, string>
            {
                { "Accept", "application/json" }
            }
        });

        return await ApiResponseMapper.FromAsync<BookingResponse>(response);
    }
}
