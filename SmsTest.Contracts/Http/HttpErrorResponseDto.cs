namespace SmsTest.Contracts.Http;

public sealed class HttpErrorResponseDto
{
    public required string Command { get; set; }
    public required bool Success { get; set; }
    public required string ErrorMessage { get; set; }
}