namespace SmsTest.Application.Abstractions;

public interface IConsole
{
    public void WriteLine(string line);
    public string ReadLine();
}