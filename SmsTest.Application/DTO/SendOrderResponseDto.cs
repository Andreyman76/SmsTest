namespace SmsTest.Application.DTO;

public sealed record SendOrderResponseDto(
    bool Success,
    string ErrorMessage
);