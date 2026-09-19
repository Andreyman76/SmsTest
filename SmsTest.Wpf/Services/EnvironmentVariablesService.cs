using Serilog;
using SmsTest.Wpf.Configuration;
using SmsTest.Wpf.Models;

namespace SmsTest.Wpf.Services;

public class EnvironmentVariablesService(
    IEnumerable<KnownVariable> knownVariables,
    ILogger? logger = default
) : IEnvironmentVariablesService
{
    private const EnvironmentVariableTarget Target =
      EnvironmentVariableTarget.User;

    private const string DefaultVariableValue = "default";

    public EnvironmentVariable[] LoadVariables()
    {
        var result = new EnvironmentVariable[knownVariables.Count()];
        var i = 0;

        foreach (var variable in knownVariables)
        {
            var value = Environment.GetEnvironmentVariable(
                variable.Name,
                Target);

            if (value is null)
            {
                value = DefaultVariableValue;

                Environment.SetEnvironmentVariable(
                    variable.Name,
                    value,
                    Target);

                logger?.Information(
                    "Environment variable {Name} initialized",
                    variable.Name);
            }

            result[i] = new EnvironmentVariable(
                Name: variable.Name,
                Value: value,
                Comment: variable.Comment);

            i++;
        }

        return result;
    }

    public void SetVariable(string name, string value)
    {
        var oldValue = Environment.GetEnvironmentVariable(
            name,
            Target);

        if (oldValue == value)
        {
            return;
        }

        Environment.SetEnvironmentVariable(
            name,
            value,
            Target);

        logger?.Information(
            "Environment variable {Name} changed",
            name);
    }
}