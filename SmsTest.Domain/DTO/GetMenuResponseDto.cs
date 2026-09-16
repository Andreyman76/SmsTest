namespace SmsTest.Domain.DTO;

public sealed record GetMenuResponseDto(
    bool Success,
    string ErrorMessage,
    MenuItemDto[] MenuItems
);