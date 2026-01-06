namespace QuantumTestSuite.Framework.API.Models;

public class HttpBinGetResponse
{
    public string? Url { get; set; }
    public IDictionary<string, string>? Headers { get; set; }
    public IDictionary<string, string>? Args { get; set; }
}
