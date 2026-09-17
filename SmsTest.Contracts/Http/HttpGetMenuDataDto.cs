namespace SmsTest.Contracts.Http;

public sealed record HttpGetMenuDataDto(
    HttpMenuItemDto[] MenuItems
);