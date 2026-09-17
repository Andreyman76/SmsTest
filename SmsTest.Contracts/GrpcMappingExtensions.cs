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
            Success: response.Success,
            ErrorMessage: response.ErrorMessage,
            MenuItems: [.. response.MenuItems.Select(ToDto)]
        );
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
            Id: item.Id,
            Article: item.Article,
            Name: item.Name,
            Price: item.Price,
            IsWeighted: item.IsWeighted,
            FullPath: item.FullPath,
            Barcodes: [.. item.Barcodes]
        );
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
            OrderId: Guid.Parse(order.Id),
            OrderItems: [.. order.OrderItems.Select(ToDto)]
        );
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
            Id: item.Id,
            Quantity: item.Quantity
        );
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
            Success: response.Success,
            ErrorMessage: response.ErrorMessage
        );
    }
}