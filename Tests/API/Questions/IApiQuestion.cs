namespace QuantumTestSuite.API.Questions;

/// <summary>
/// Represents a question that can be asked about API responses.
/// Questions are used to retrieve information and perform assertions on API data.
/// </summary>
/// <typeparam name="T">The type of answer this question returns</typeparam>
public interface IApiQuestion<T>
{
    /// <summary>
    /// Asks the question about the API response and returns the answer
    /// </summary>
    /// <param name="actor">The actor asking the question</param>
    /// <returns>The answer to the question</returns>
    Task<T> AnsweredBy(UI.Screenplay.Actors.Actor actor);
}
