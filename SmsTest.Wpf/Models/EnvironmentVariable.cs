namespace SmsTest.Wpf.Models;

public sealed record EnvironmentVariable(
    string Name,
    string Value,
    string? Comment
);