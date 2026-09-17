namespace SmsTest.Wpf.Configuration;

public sealed record KnownVariable
{
    public string Name { get; set; } = string.Empty;
    public string? Comment { get; set; }
}