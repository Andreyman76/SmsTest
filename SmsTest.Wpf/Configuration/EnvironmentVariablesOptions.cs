namespace SmsTest.Wpf.Configuration;

internal sealed class EnvironmentVariablesOptions
{
    public const string SectionName = "EnvironmentVariables";

    public KnownVariable[] KnownVariables { get; init; } = [];
}