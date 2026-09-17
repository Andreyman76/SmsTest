using Serilog;

namespace SmsTest.ConsoleApp.Utilities;

/// <summary>
/// Утилита, совмещающая в себе чтение/запись в консоль с ведением логов того, что было прочитано/записано
/// </summary>
/// <param name="logger"></param>
internal class LoggedConsole(
    ILogger logger)
{
    public void WriteLine(string line)
    {
        Console.WriteLine(line);

        logger.Information("WRITE {line}", line);
    }

    public string ReadLine()
    {
        var line = Console.ReadLine()
            ?? throw new InvalidOperationException("User iput is null");

        logger.Information("READ {line}", line);

        return line;
    }
}