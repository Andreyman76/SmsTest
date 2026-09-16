namespace SmsTest.Contracts.Http;

public sealed class HttpSendOrderResponseDto
{
    public string Command { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}