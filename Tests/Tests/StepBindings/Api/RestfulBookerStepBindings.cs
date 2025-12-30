using Allure.NUnit.Attributes;
using NUnit.Framework;
using QuantumTestSuite.API.Models;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Core.Reporting;
using QuantumTestSuite.Core.Services;
using QuantumTestSuite.Tests.StepBindings.Base;
using Reqnroll;

namespace QuantumTestSuite.Tests.StepBindings.Api;

/// <summary>
/// Step bindings for Restful Booker API tests
/// </summary>
[Binding]
[AllureParentSuite("API Tests")]
[AllureSuite("Restful Booker")]
public class RestfulBookerStepBindings : ApiStepBindingsBase
{
    private readonly IBookingService _bookingService;

    public RestfulBookerStepBindings(
        ScenarioContext scenarioContext,
        AppSettings settings,
        IBookingService bookingService)
        : base(scenarioContext, settings)
    {
        _bookingService = bookingService;
    }

    [Given(@"un payload válido de booking")]
    public Task GivenUnPayloadValidoDeBooking()
    {
        Context.BookingPayload = new BookingRequest
        {
            Firstname = "Chadd",
            Lastname = "Gutkowski",
            Totalprice = 389,
            Depositpaid = false,
            Bookingdates = new BookingDates
            {
                Checkin = "2026-01-01",
                Checkout = "2026-01-11"
            },
            Additionalneeds = "Breakfast"
        };

        // Attach payload for visibility
        AllureHelper.AttachJson("booking-payload", 
            System.Text.Json.JsonSerializer.Serialize(Context.BookingPayload));

        return Task.CompletedTask;
    }

    [When(@"se envía POST \/booking")]
    public async Task WhenSeEnviaPostBooking()
    {
        var booking = Context.BookingPayload!;

        var response = await ExecuteApiCallAsync(
            "POST",
            $"{Settings.Apis.RestfulBooker}/booking",
            () => _bookingService.CreateBookingAsync(booking),
            requestBody: booking);

        Context.BookingResponse = response;
    }

    [Then(@"el response contiene bookingid")]
    public void ThenElResponseContieneBookingid()
    {
        Assert.That(Context.BookingResponse?.Data?.Bookingid, Is.GreaterThan(0),
            "Booking ID should be greater than 0");
        Assert.That(Context.BookingResponse?.Data?.Booking?.Firstname, Is.Not.Empty,
            "Firstname should not be empty");
    }
}
