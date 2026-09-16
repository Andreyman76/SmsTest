namespace SmsTest.Domain.DTO;

public sealed record SendOrderResponseDto(
    bool Success,
    string ErrorMessage
);