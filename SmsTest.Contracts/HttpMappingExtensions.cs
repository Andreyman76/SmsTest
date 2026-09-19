using SmsTest.Application.DTO;
using SmsTest.Contracts.Http;
using System.Globalization;

namespace SmsTest.Contracts;

/// <summary>
/// <para>Методы расширения для маппинга сущностей HTTP</para>
/// <para>Решено не использовать AutoMapper для столь небольшого проекта</para>
/// </summary>
public static class HttpMappingExtensions
{
    public static HttpGetMenuRequestDto ToHttpDto(
        this GetMenuRequestDto request)
    {
        return new HttpGetMenuRequestDto(
            WithPrice: request.WithPrice
        );
    }

    public static GetMenuRequestDto ToDto(
        this HttpGetMenuRequestDto request)
    {
        return new GetMenuRequestDto(
            WithPrice: request.WithPrice
        );
    }

    public static HttpGetMenuResponseDto ToHttpDto(
        this GetMenuResponseDto response)
    {
        return new HttpGetMenuResponseDto(
            Command: SmsTestApiCommands.GetMenuCommand,
            Success: response.Success,
            ErrorMessage: response.ErrorMessage,
            Data: new HttpGetMenuDataDto(
                MenuItems: [.. response.MenuItems
                    .Select(ToHttpDto)]
            )
        );
    }

    public static GetMenuResponseDto ToDto(
        this HttpGetMenuResponseDto response)
    {
        var items = response.Data?.MenuItems
                .Select(ToDto) ?? [];

        return new GetMenuResponseDto(
            response.Success,
            response.ErrorMessage,
            [.. items]
        );
    }

    public static HttpMenuItemDto ToHttpDto(
        this MenuItemDto item)
    {
        return new HttpMenuItemDto(
            Id: item.Id,
            Article: item.Article,
            Name: item.Name,
            Price: item.Price,
            IsWeighted: item.IsWeighted,
            FullPath: item.FullPath,
            Barcodes: item.Barcodes
        );
    }

    public static MenuItemDto ToDto(
        this HttpMenuItemDto item)
    {
        return new MenuItemDto(
            Id: item.Id,
            Article: item.Article,
            Name: item.Name,
            Price: item.Price,
            IsWeighted: item.IsWeighted,
            FullPath: item.FullPath,
            Barcodes: item.Barcodes
        );
    }

    public static HttpSendOrderRequestDto ToHttpDto(
        this SendOrderRequestDto request)
    {
        return new HttpSendOrderRequestDto(
            OrderId: request.OrderId,
            MenuItems: [.. request.OrderItems
                .Select(ToHttpDto)]
        );
    }

    public static SendOrderRequestDto ToDto(
        this HttpSendOrderRequestDto order)
    {
        return new SendOrderRequestDto(
            OrderId: order.OrderId,
            OrderItems: [.. order.MenuItems
                .Select(ToDto)]
        );
    }

    public static HttpOrderItemDto ToHttpDto(
        this OrderItemDto request)
    {
        return new HttpOrderItemDto(
            Id: request.Id,
            Quantity: request.Quantity.ToString(
                CultureInfo.InvariantCulture)
        );
    }

    public static OrderItemDto ToDto(
        this HttpOrderItemDto item)
    {
        return new OrderItemDto(
            Id: item.Id,
            Quantity: double.Parse(
                item.Quantity,
                CultureInfo.InvariantCulture)
        );
    }

    public static HttpSendOrderResponseDto ToHttpDto(
        this SendOrderResponseDto response)
    {
        return new HttpSendOrderResponseDto(
            Command: SmsTestApiCommands.SendOrderCommand,
            Success: response.Success,
            ErrorMessage: response.ErrorMessage
        );
    }

    public static SendOrderResponseDto ToDto(
        this HttpSendOrderResponseDto response)
    {
        return new SendOrderResponseDto(
            Success: response.Success,
            ErrorMessage: response.ErrorMessage
        );
    }
}