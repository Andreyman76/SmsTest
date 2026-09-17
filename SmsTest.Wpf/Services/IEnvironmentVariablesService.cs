using SmsTest.Wpf.Models;

namespace SmsTest.Wpf.Services;

/// <summary>
/// Сервис для работы с заранее известными переменными среды
/// </summary>
public interface IEnvironmentVariablesService
{
    EnvironmentVariable[] GetAllVariables();
    void SetVariable(string name, string value);
}