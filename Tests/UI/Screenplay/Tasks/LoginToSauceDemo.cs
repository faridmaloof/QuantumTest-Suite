using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Actors;

namespace QuantumTestSuite.UI.Screenplay.Tasks;

public class LoginToSauceDemo : ITask
{
    private readonly string _username;
    private readonly string _password;

    public LoginToSauceDemo(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public async Task ExecuteAsync(Actor actor)
    {
        var page = new SauceDemoLoginPage(actor.Page);
        await page.NavigateAsync();
        await page.LoginAsync(_username, _password);
    }
}
