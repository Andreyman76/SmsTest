namespace SmsTest.Contracts.Http;

public sealed record HttpGetMenuResponseDto(
    string Command,
    bool Success,
    string ErrorMessage,
    HttpGetMenuDataDto? Data
);