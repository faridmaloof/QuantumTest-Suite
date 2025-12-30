using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Actors;

namespace QuantumTestSuite.UI.Screenplay.Tasks;

public class SearchGitHubUser : ITask
{
    private readonly string _baseUrl;
    private readonly string _username;

    public SearchGitHubUser(string baseUrl, string username)
    {
        _baseUrl = baseUrl;
        _username = username;
    }

    public async Task ExecuteAsync(Actor actor)
    {
        var page = new GhUsersSearchPage(actor.Page);
        await page.NavigateAsync(_baseUrl);
        await page.SearchAsync(_username);
    }
}
