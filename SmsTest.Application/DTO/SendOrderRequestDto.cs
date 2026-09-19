namespace SmsTest.Application.DTO;

public sealed record SendOrderRequestDto(
    Guid OrderId,
    OrderItemDto[] OrderItems
);