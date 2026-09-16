using Sms.Test;
using SmsTest.Domain.DTO;

namespace SmsTest.Contracts;

public static class GrpcMappingExtensions
{
    public static GetMenuResponse ToProto(this GetMenuResponseDto response)
    {
        var result = new GetMenuResponse
        {
            Success = response.Success,
            ErrorMessage = response.ErrorMessage,
        };

        result.MenuItems.AddRange(
            response.MenuItems.Select(ToProto));

        return result;
    }

    public static GetMenuResponseDto ToDto(this GetMenuResponse response)
    {
        return new GetMenuResponseDto(
            response.Success,
            response.ErrorMessage,
            [.. response.MenuItems.Select(ToDto)]);
    }

    public static MenuItem ToProto(this MenuItemDto menuItem)
    {
        var result = new MenuItem
        {
            Id = menuItem.Id,
            Article = menuItem.Article,
            Name = menuItem.Name,
            Price = menuItem.Price,
            IsWeighted = menuItem.IsWeighted,
            FullPath = menuItem.FullPath,
        };

        result.Barcodes.AddRange(menuItem.Barcodes);

        return result;
    }

    public static MenuItemDto ToDto(this MenuItem item)
    {
        return new MenuItemDto(
            item.Id,
            item.Article,
            item.Name,
            item.Price,
            item.IsWeighted,
            item.FullPath,
            [.. item.Barcodes]);
    }

    public static Order ToProto(this SendOrderRequestDto order)
    {
        var result = new Order
        {
            Id = order.OrderId.ToString()
        };

        result.OrderItems.AddRange(
            order.OrderItems.Select(ToProto));

        return result;
    }

    public static SendOrderRequestDto ToDto(this Order order)
    {
        return new SendOrderRequestDto(
            Guid.Parse(order.Id),
            [.. order.OrderItems.Select(ToDto)]);
    }

    public static OrderItem ToProto(this OrderItemDto orderItem)
    {
        return new OrderItem
        {
            Id = orderItem.Id,
            Quantity = orderItem.Quantity
        };
    }

    public static OrderItemDto ToDto(this OrderItem item)
    {
        return new OrderItemDto(
            item.Id,
            item.Quantity);
    }

    public static SendOrderResponse ToProto(this SendOrderResponseDto response)
    {
        return new SendOrderResponse
        {
            Success = response.Success,
            ErrorMessage = response.ErrorMessage
        };
    }

    public static SendOrderResponseDto ToDto(this SendOrderResponse response)
    {
        return new SendOrderResponseDto(
            response.Success,
            response.ErrorMessage);
    }
}