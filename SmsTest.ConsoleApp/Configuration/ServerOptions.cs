namespace SmsTest.ConsoleApp.Configuration;

internal sealed class ServerOptions
{
    public const string SectionName = "Server";

    public required string Protocol { get; init; }
    public string GrpcServerUrl { get; init; } = string.Empty;
    public string HttpServerUrl { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}