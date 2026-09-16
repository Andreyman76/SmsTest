using SmsTest.Domain.DTO;

namespace SmsTest.Server.Services;

internal class SmsTestServiceMockup
{
    private readonly MenuItemDto[] _menu =
    [
        new MenuItemDto(
            "5979224",
            "A1004292",
            "Каша гречневая",
            50,
            false,
            "ПРОИЗВОДСТВО\\Гарниры",
            ["57890975627974236429"]),
        new MenuItemDto(
            "9084246",
            "A1004293",
            "Конфеты Коровка",
            300,
            true,
            "ДЕСЕРТЫ\\Развес",
            [])
    ];

    public async Task<GetMenuResponseDto> GetMenuAsync(
        GetMenuRequestDto request,
        CancellationToken token = default)
    {
        return new GetMenuResponseDto(
            true,
            string.Empty,
            request.WithPrice
            ? _menu.ToArray()
            : _menu.Select(x => new MenuItemDto(
                x.Id,
                x.Article,
                x.Name,
                default, // Без цены
                x.IsWeighted,
                x.FullPath,
                x.Barcodes
            )).ToArray());
    }

    public async Task<SendOrderResponseDto> SendOrderAsync(
        SendOrderRequestDto order,
        CancellationToken token = default)
    {
        foreach (var item in order.OrderItems)
        {
            if (!_menu.Any(x => x.Id == item.Id))
            {
                return new SendOrderResponseDto(
                    false,
                    $"Блюда с id {item.Id} нет в меню");
            }

            if (item.Quantity <= 0.0)
            {
                return new SendOrderResponseDto(
                    false,
                    $"Для блюда с id {item.Id} указано неверное количество {item.Quantity}, ожидалось больше нуля");
            }
        }

        return new SendOrderResponseDto(
            true,
            string.Empty);
    }
}