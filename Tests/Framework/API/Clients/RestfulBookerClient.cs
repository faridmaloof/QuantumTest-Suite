using System.Text.Json;
using Microsoft.Playwright;
using QuantumTestSuite.Framework.API.Helpers;
using QuantumTestSuite.Framework.API.Models;
using QuantumTestSuite.Framework.Core.Reporting;

namespace QuantumTestSuite.Framework.API.Clients;

public class RestfulBookerClient(IAPIRequestContext context)
{
    private readonly IAPIRequestContext _context = context;

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
