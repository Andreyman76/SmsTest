using SmsTest.Wpf.Models;

namespace SmsTest.Wpf.Services;

/// <summary>
/// Сервис для работы с заранее известными переменными среды
/// </summary>
public interface IEnvironmentVariablesService
{
    EnvironmentVariable[] LoadVariables();
    void SetVariable(string name, string value);
}