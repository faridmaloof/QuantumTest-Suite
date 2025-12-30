namespace QuantumTestSuite.API.Models;

public class BookingDates
{
    public string Checkin { get; set; } = string.Empty;
    public string Checkout { get; set; } = string.Empty;
}

public class BookingRequest
{
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public int Totalprice { get; set; }
    public bool Depositpaid { get; set; }
    public BookingDates Bookingdates { get; set; } = new();
    public string Additionalneeds { get; set; } = string.Empty;
}

public class BookingResponse
{
    public int Bookingid { get; set; }
    public BookingRequest? Booking { get; set; }
}
