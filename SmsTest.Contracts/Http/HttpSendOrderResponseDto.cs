namespace SmsTest.Contracts.Http;

public sealed record HttpSendOrderResponseDto(
    string Command,
    bool Success,
    string ErrorMessage
);