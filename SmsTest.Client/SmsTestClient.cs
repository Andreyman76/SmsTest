using SmsTest.Domain;
using SmsTest.Domain.DTO;

namespace SmsTest.Client;

public class SmsTestClient(ISmsTestServiceClient client)
{
    public async Task<MenuItemDto[]> GetMenuAsync(
        bool withPrice,
        CancellationToken token = default)
    {
        var response = await client.GetMenuAsync(new GetMenuRequestDto(withPrice), token);

        if (!response.Success)
        {
            throw new InvalidOperationException($"Сервер вернул ошибку на запрос получения меню: {response.ErrorMessage}");
        }

        return response.MenuItems;
    }

    public async Task SendOrderAsync(
        OrderItemDto[] orderItems,
        CancellationToken token = default)
    {
        var response = await client.SendOrderAsync(
            new SendOrderRequestDto(
                Guid.NewGuid(),
                orderItems),
            token);

        if (!response.Success)
        {
            throw new InvalidOperationException($"Сервер вернул ошибку на запрос создания заказа: {response.ErrorMessage}");
        }
    }
}
