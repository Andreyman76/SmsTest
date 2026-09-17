namespace SmsTest.Contracts.Http;

public sealed record HttpErrorResponseDto(
    string Command,
    bool Success,
    string ErrorMessage
);