namespace SmsTest.Server.Configuration;

internal class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    public required string Username { get; init; }
    public required string Password { get; init; }
}