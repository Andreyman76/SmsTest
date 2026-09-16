using Serilog;

namespace SmsTest.ConsoleApp.Utilities;

internal static class ConsoleLogger
{
    public static void WriteLine(string line)
    {
        Console.WriteLine(line);
        Log.Information("WRITE {line}", line);
    }

    public static string ReadLine()
    {
        var line = Console.ReadLine()
            ?? throw new InvalidOperationException("User iput is null");
        Log.Information("READ {line}", line);

        return line;
    }
}