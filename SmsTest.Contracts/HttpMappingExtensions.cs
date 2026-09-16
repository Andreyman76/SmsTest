using SmsTest.Contracts.Http;
using SmsTest.Domain.DTO;
using System.Globalization;

namespace SmsTest.Contracts;

public static class HttpMappingExtensions
{
    public static HttpGetMenuRequestDto ToHttpDto(this GetMenuRequestDto request)
    {
        return new HttpGetMenuRequestDto
        {
            WithPrice = request.WithPrice,
        };
    }

    public static GetMenuRequestDto ToDto(this HttpGetMenuRequestDto request)
    {
        return new GetMenuRequestDto(request.WithPrice);
    }

    public static HttpGetMenuResponseDto ToHttpDto(this GetMenuResponseDto response)
    {
        return new HttpGetMenuResponseDto
        {
            Command = SmsTestApiCommands.GetMenuCommand,
            Success = response.Success,
            ErrorMessage = response.ErrorMessage,
            Data = new()
            {
                MenuItems = [.. response.MenuItems
                    .Select(ToHttpDto)]
            }
        };
    }

    public static GetMenuResponseDto ToDto(this HttpGetMenuResponseDto response)
    {
        return new GetMenuResponseDto(
            response.Success,
            response.ErrorMessage,
            [.. response.Data.MenuItems
                .Select(ToDto)]
        );
    }

    public static HttpMenuItemDto ToHttpDto(this MenuItemDto item)
    {
        return new HttpMenuItemDto
        {
            Id = item.Id,
            Article = item.Article,
            Name = item.Name,
            Price = item.Price,
            IsWeighted = item.IsWeighted,
            FullPath = item.FullPath,
            Barcodes = item.Barcodes
        };
    }

    public static MenuItemDto ToDto(this HttpMenuItemDto item)
    {
        return new MenuItemDto(
            item.Id,
            item.Article,
            item.Name,
            item.Price,
            item.IsWeighted,
            item.FullPath,
            item.Barcodes
        );
    }

    public static HttpSendOrderRequestDto ToHttpDto(this SendOrderRequestDto request)
    {
        return new HttpSendOrderRequestDto
        {
            OrderId = request.OrderId,
            MenuItems = [.. request.OrderItems
                .Select(ToHttpDto)]
        };
    }

    public static SendOrderRequestDto ToDto(this HttpSendOrderRequestDto order)
    {
        return new SendOrderRequestDto(
            order.OrderId,
            [.. order.MenuItems
                .Select(ToDto)]);
    }

    public static HttpOrderItem ToHttpDto(this OrderItemDto request)
    {
        return new HttpOrderItem
        {
            Id = request.Id,
            Quantity = request.Quantity.ToString(CultureInfo.InvariantCulture)
        };
    }

    public static OrderItemDto ToDto(this HttpOrderItem item)
    {
        return new OrderItemDto(
            item.Id,
            double.Parse(
                item.Quantity,
                CultureInfo.InvariantCulture));
    }

    public static HttpSendOrderResponseDto ToHttpDto(this SendOrderResponseDto response)
    {
        return new HttpSendOrderResponseDto
        {
            Command = SmsTestApiCommands.SendOrderCommand,
            Success = response.Success,
            ErrorMessage = response.ErrorMessage
        };
    }

    public static SendOrderResponseDto ToDto(this HttpSendOrderResponseDto response)
    {
        return new SendOrderResponseDto(
            response.Success,
            response.ErrorMessage
        );
    }
}