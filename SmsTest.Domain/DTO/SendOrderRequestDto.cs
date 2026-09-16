namespace SmsTest.Domain.DTO;

public sealed record SendOrderRequestDto(
    Guid OrderId,
    OrderItemDto[] OrderItems
);