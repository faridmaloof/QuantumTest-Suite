using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using QuantumTestSuite.Data.Factories;
using QuantumTestSuite.API.Models;
using Reqnroll;
using System.Text;

namespace QuantumTestSuite.Tests.StepBindings.UnitFeatures;

[Binding]
[AllureParentSuite("Unit Features")]
[AllureSuite("Data")]
[AllureSubSuite("Test Data Factories")]
public class TestDataFactoryStepBindings
{
    private readonly ScenarioContext _scenarioContext;
    private BookingRequest? _generatedBooking;
    private List<BookingRequest> _generatedBookings = new();

    public TestDataFactoryStepBindings(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"las factories están inicializadas")]
    [AllureStep("Factories inicializadas")]
    public void GivenFactoriesInicializadas()
    {
        // BookingFactory es estático, siempre disponible
        AllureApi.AddAttachment("Info", "text/plain", Encoding.UTF8.GetBytes("BookingFactory lista para uso"), ".txt");
    }

    [When(@"se genera un booking usando BookingFactory")]
    [AllureStep("Generar booking con BookingFactory")]
    public void WhenSeGeneraUnBooking()
    {
        _generatedBooking = BookingFactory.CreateBookingRequest();
        Assert.That(_generatedBooking, Is.Not.Null, "Booking generado no debe ser null");
        
        AllureApi.AddAttachment("Booking generado", "application/json", 
            Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(_generatedBooking, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })), ".txt");
    }

    [Then(@"el booking debe tener firstname no vacío")]
    [AllureStep("Verificar firstname no vacío")]
    public void ThenBookingTieneFirstnameNoVacio()
    {
        Assert.That(_generatedBooking, Is.Not.Null);
        Assert.That(_generatedBooking.Firstname, Is.Not.Null.And.Not.Empty,
            "Firstname no debe estar vacío");
    }

    [Then(@"el booking debe tener lastname no vacío")]
    [AllureStep("Verificar lastname no vacío")]
    public void ThenBookingTieneLastnameNoVacio()
    {
        Assert.That(_generatedBooking, Is.Not.Null);
        Assert.That(_generatedBooking.Lastname, Is.Not.Null.And.Not.Empty,
            "Lastname no debe estar vacío");
    }

    [Then(@"el booking debe tener totalprice mayor a 0")]
    [AllureStep("Verificar totalprice > 0")]
    public void ThenBookingTieneTotalpriceMayorCero()
    {
        Assert.That(_generatedBooking, Is.Not.Null);
        Assert.That(_generatedBooking.Totalprice, Is.GreaterThan(0),
            $"Totalprice debe ser mayor a 0, actual: {_generatedBooking.Totalprice}");
    }

    [Then(@"el booking debe tener depositpaid como booleano válido")]
    [AllureStep("Verificar depositpaid es booleano")]
    public void ThenBookingTieneDepositpaidValido()
    {
        Assert.That(_generatedBooking, Is.Not.Null);
        Assert.That(_generatedBooking.Depositpaid, Is.TypeOf<bool>(),
            "Depositpaid debe ser booleano");
    }

    [Then(@"bookingdates debe tener checkin y checkout válidos")]
    [AllureStep("Verificar bookingdates tienen valores válidos")]
    public void ThenBookingdatesTieneCheckinCheckoutValidos()
    {
        Assert.That(_generatedBooking, Is.Not.Null);
        Assert.That(_generatedBooking.Bookingdates, Is.Not.Null, "Bookingdates no debe ser null");
        Assert.That(_generatedBooking.Bookingdates.Checkin, Is.Not.Null.And.Not.Empty, "Checkin no debe estar vacío");
        Assert.That(_generatedBooking.Bookingdates.Checkout, Is.Not.Null.And.Not.Empty, "Checkout no debe estar vacío");
    }

    [Then(@"checkin debe ser anterior a checkout")]
    [AllureStep("Verificar checkin < checkout")]
    public void ThenCheckinAnteriorCheckout()
    {
        Assert.That(_generatedBooking, Is.Not.Null);
        
        var checkin = DateTime.Parse(_generatedBooking.Bookingdates.Checkin);
        var checkout = DateTime.Parse(_generatedBooking.Bookingdates.Checkout);
        
        Assert.That(checkin, Is.LessThan(checkout),
            $"Checkin ({checkin:yyyy-MM-dd}) debe ser anterior a Checkout ({checkout:yyyy-MM-dd})");
        
        AllureApi.AddAttachment("Fechas", "text/plain", Encoding.UTF8.GetBytes($"Checkin: {checkin:yyyy-MM-dd}\nCheckout: {checkout:yyyy-MM-dd}"), ".txt");
    }

    [When(@"se generan (\d+) bookings usando BookingFactory")]
    [AllureStep("Generar {count} bookings")]
    public void WhenSeGeneranMultiplesBookings(int count)
    {
        _generatedBookings.Clear();
        
        for (int i = 0; i < count; i++)
        {
            _generatedBookings.Add(BookingFactory.CreateBookingRequest());
        }
        
        Assert.That(_generatedBookings, Has.Count.EqualTo(count),
            $"Deben generarse {count} bookings");
        
        AllureApi.AddAttachment("Bookings generados", "text/plain", Encoding.UTF8.GetBytes($"Total: {_generatedBookings.Count}"), ".txt");
    }

    [Then(@"todos los bookings deben tener diferentes firstnames")]
    [AllureStep("Verificar firstnames únicos")]
    public void ThenTodosBookingsTienenDiferentesFirstnames()
    {
        var firstnames = _generatedBookings.Select(b => b.Firstname).ToList();
        var uniqueFirstnames = firstnames.Distinct().ToList();
        
        Assert.That(uniqueFirstnames, Has.Count.EqualTo(firstnames.Count),
            "Todos los firstnames deben ser únicos");
        
        AllureApi.AddAttachment("Firstnames", "text/plain", 
            Encoding.UTF8.GetBytes(string.Join("\n", firstnames)), ".txt");
    }

    [Then(@"todos los bookings deben tener diferentes lastnames")]
    [AllureStep("Verificar lastnames únicos")]
    public void ThenTodosBookingsTienenDiferentesLastnames()
    {
        var lastnames = _generatedBookings.Select(b => b.Lastname).ToList();
        var uniqueLastnames = lastnames.Distinct().ToList();
        
        Assert.That(uniqueLastnames, Has.Count.EqualTo(lastnames.Count),
            "Todos los lastnames deben ser únicos");
    }

    [When(@"se genera un booking con firstname ""(.*)"" usando BookingFactory")]
    [AllureStep("Generar booking con firstname personalizado: {firstname}")]
    public void WhenSeGeneraBookingConFirstname(string firstname)
    {
        _generatedBooking = BookingFactory.CreateBookingRequest(firstname: firstname);
        Assert.That(_generatedBooking, Is.Not.Null);
    }

    [Then(@"el booking debe tener firstname ""(.*)""")]
    [AllureStep("Verificar firstname = '{expected}'")]
    public void ThenBookingTieneFirstname(string expected)
    {
        Assert.That(_generatedBooking, Is.Not.Null);
        Assert.That(_generatedBooking.Firstname, Is.EqualTo(expected),
            $"Firstname debe ser {expected} pero es {_generatedBooking.Firstname}");
    }

    [Then(@"los demás campos deben ser generados automáticamente")]
    [AllureStep("Verificar campos auto-generados")]
    public void ThenDemasCamposGeneradosAutomaticamente()
    {
        Assert.That(_generatedBooking, Is.Not.Null);
        Assert.That(_generatedBooking.Lastname, Is.Not.Null.And.Not.Empty);
        Assert.That(_generatedBooking.Totalprice, Is.GreaterThan(0));
        Assert.That(_generatedBooking.Bookingdates, Is.Not.Null);
    }

    [Then(@"todos los precios deben estar entre (\d+) y (\d+)")]
    [AllureStep("Verificar precios en rango {min} - {max}")]
    public void ThenTodosPreciosEnRango(int min, int max)
    {
        foreach (var booking in _generatedBookings)
        {
            Assert.That(booking.Totalprice, Is.InRange(min, max),
                $"Precio {booking.Totalprice} debe estar entre {min} y {max}");
        }
        
        var prices = _generatedBookings.Select(b => b.Totalprice).ToList();
        AllureApi.AddAttachment("Precios generados", "text/plain", 
            Encoding.UTF8.GetBytes($"Min: {prices.Min()}\nMax: {prices.Max()}\nPromedio: {prices.Average():F2}"), ".txt");
    }

    [Then(@"todas las fechas de checkout deben ser después de checkin")]
    [AllureStep("Verificar checkout > checkin en todos los bookings")]
    public void ThenTodasFechasCheckoutDespuesCheckin()
    {
        foreach (var booking in _generatedBookings)
        {
            var checkin = DateTime.Parse(booking.Bookingdates.Checkin);
            var checkout = DateTime.Parse(booking.Bookingdates.Checkout);
            
            Assert.That(checkout, Is.GreaterThan(checkin),
                $"Checkout debe ser posterior a Checkin");
        }
    }

    [Then(@"todas las fechas deben ser en el futuro")]
    [AllureStep("Verificar fechas futuras")]
    public void ThenTodasFechasFuturo()
    {
        var today = DateTime.Today;
        
        foreach (var booking in _generatedBookings)
        {
            var checkin = DateTime.Parse(booking.Bookingdates.Checkin);
            
            Assert.That(checkin, Is.GreaterThanOrEqualTo(today),
                $"Checkin debe ser hoy o en el futuro, pero es {checkin:yyyy-MM-dd}");
        }
    }
}
