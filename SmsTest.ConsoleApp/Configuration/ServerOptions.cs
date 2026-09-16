namespace SmsTest.ConsoleApp.Configuration;

internal sealed class ServerOptions
{
    public const string SectionName = "Server";
    public required string GrpcServerUrl { get; init; }
    public required string HttpServerUrl { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
}