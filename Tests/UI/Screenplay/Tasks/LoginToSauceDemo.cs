using QuantumTestSuite.Core.Config;
using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Actors;

namespace QuantumTestSuite.UI.Screenplay.Tasks;

public class LoginToSauceDemo : ITask
{
    private readonly string _username;
    private readonly string _password;
    private readonly AppSettings _settings;

    public LoginToSauceDemo(string username, string password, AppSettings settings)
    {
        _username = username;
        _password = password;
        _settings = settings;
    }

    public async Task ExecuteAsync(Actor actor)
    {
        var page = new SauceDemoLoginPage(actor.Page, _settings);
        await page.NavigateAsync();
        await page.LoginAsync(_username, _password);
    }
}
