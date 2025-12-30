using Bogus;
using QuantumTestSuite.API.Models;

namespace QuantumTestSuite.Data.Factories;

public static class BookingFactory
{
    public static BookingRequest Create()
    {
        var faker = new Faker();
        var checkin = faker.Date.SoonOffset(2);
        var checkout = checkin.AddDays(faker.Random.Int(3, 10));
        
        return new BookingRequest
        {
            Firstname = faker.Name.FirstName(),
            Lastname = faker.Name.LastName(),
            Totalprice = faker.Random.Int(100, 500),
            Depositpaid = faker.Random.Bool(),
            Bookingdates = new BookingDates
            {
                Checkin = checkin.ToString("yyyy-MM-dd"),
                Checkout = checkout.ToString("yyyy-MM-dd")
            },
            Additionalneeds = "Breakfast"
        };
    }

    public static BookingRequest CreateBookingRequest(
        string? firstname = null,
        string? lastname = null,
        int? totalprice = null,
        bool? depositpaid = null,
        string? checkin = null,
        string? checkout = null,
        string? additionalneeds = null)
    {
        var faker = new Faker();
        var checkinDate = checkin != null ? DateTimeOffset.Parse(checkin) : faker.Date.SoonOffset(2);
        var checkoutDate = checkout != null ? DateTimeOffset.Parse(checkout) : checkinDate.AddDays(faker.Random.Int(3, 10));
        
        return new BookingRequest
        {
            Firstname = firstname ?? faker.Name.FirstName(),
            Lastname = lastname ?? faker.Name.LastName(),
            Totalprice = totalprice ?? faker.Random.Int(100, 500),
            Depositpaid = depositpaid ?? faker.Random.Bool(),
            Bookingdates = new BookingDates
            {
                Checkin = checkinDate.ToString("yyyy-MM-dd"),
                Checkout = checkoutDate.ToString("yyyy-MM-dd")
            },
            Additionalneeds = additionalneeds ?? "Breakfast"
        };
    }
}
