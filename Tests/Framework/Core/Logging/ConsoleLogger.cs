namespace QuantumTestSuite.Framework.Core.Logging;

public static class ConsoleLogger
{
    public static void Info(string message) => Write("INFO", message);
    public static void Warn(string message) => Write("WARN", message);
    public static void Error(string message) => Write("ERROR", message);

    private static void Write(string level, string message)
    {
        var formatted = $"[{DateTime.UtcNow:O}] [{level}] {message}";
        Console.WriteLine(formatted);
    }
}
