using Bogus;

namespace QuantumTestSuite.Data.Factories;

public static class UserFactory
{
    public static (string username, string password) CreateSauceUser()
    {
        var faker = new Faker();
        return ($"user_{faker.Random.AlphaNumeric(5)}", faker.Internet.Password());
    }
}
