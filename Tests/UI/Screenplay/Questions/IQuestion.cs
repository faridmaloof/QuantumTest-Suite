namespace QuantumTestSuite.UI.Screenplay.Questions;

/// <summary>
/// Represents a question that can be asked about the system under test.
/// Questions are used to retrieve information and perform assertions in the Screenplay Pattern.
/// </summary>
/// <typeparam name="T">The type of answer this question returns</typeparam>
public interface IQuestion<T>
{
    /// <summary>
    /// Asks the question using the actor's abilities and returns the answer
    /// </summary>
    /// <param name="actor">The actor asking the question</param>
    /// <returns>The answer to the question</returns>
    Task<T> AnsweredBy(Actors.Actor actor);
}
