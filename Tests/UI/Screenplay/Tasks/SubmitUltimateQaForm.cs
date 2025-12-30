using QuantumTestSuite.UI.Pages;
using QuantumTestSuite.UI.Screenplay.Actors;

namespace QuantumTestSuite.UI.Screenplay.Tasks;

public class SubmitUltimateQaForm : ITask
{
    private readonly string _name;
    private readonly string _email;
    private readonly string _message;

    public SubmitUltimateQaForm(string name, string email, string message)
    {
        _name = name;
        _email = email;
        _message = message;
    }

    public async Task ExecuteAsync(Actor actor)
    {
        var page = new UltimateQaFormPage(actor.Page);
        await page.NavigateAsync();
        await page.SubmitFormAsync(_name, _email, _message);
    }
}
