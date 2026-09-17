namespace SmsTest.Contracts.Http;

public sealed record HttpSendOrderRequestDto(
    Guid OrderId,
    HttpOrderItemDto[] MenuItems
);