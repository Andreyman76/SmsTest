using SmsTest.Domain.DTO;

namespace SmsTest.Server.Services;

/// <summary>
/// Тестовая реализация сервиса
/// </summary>
internal class SmsTestServiceMockup
    : ISmsTestService
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
            []),
        new MenuItemDto(
            "3147821",
            "A1004294",
            "Картофельное пюре",
            75,
            false,
            "ПРОИЗВОДСТВО\\Гарниры",
            ["57890975627974236430"]),
        new MenuItemDto(
            "6729145",
            "A1004295",
            "Котлета куриная",
            120,
            false,
            "ПРОИЗВОДСТВО\\Горячие блюда",
            ["57890975627974236431"]),
        new MenuItemDto(
            "8215637",
            "A1004296",
            "Салат Оливье",
            420,
            true,
            "САЛАТЫ\\Развес",
            []),
        new MenuItemDto(
            "4592386",
            "A1004297",
            "Суп куриный с лапшой",
            180,
            false,
            "ПРОИЗВОДСТВО\\Первые блюда",
            ["57890975627974236432"]),
        new MenuItemDto(
            "7351942",
            "A1004298",
            "Борщ со сметаной",
            220,
            false,
            "ПРОИЗВОДСТВО\\Первые блюда",
            ["57890975627974236433"]),
        new MenuItemDto(
            "2864719",
            "A1004299",
            "Сыр Российский",
            650,
            true,
            "ПРОДУКТЫ\\Молочные продукты",
            []),
        new MenuItemDto(
            "9436158",
            "A1004300",
            "Булочка с маком",
            65,
            false,
            "ВЫПЕЧКА\\Сдоба",
            ["57890975627974236434"]),
        new MenuItemDto(
            "5183724",
            "A1004301",
            "Пирожное Наполеон",
            190,
            false,
            "ДЕСЕРТЫ\\Пирожные",
            ["57890975627974236435"])
    ];

    public Task<GetMenuResponseDto> GetMenuAsync(
        GetMenuRequestDto request,
        CancellationToken token = default)
    {
        var response = new GetMenuResponseDto(
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

        return Task.FromResult(response);
    }

    public Task<SendOrderResponseDto> SendOrderAsync(
        SendOrderRequestDto order,
        CancellationToken token = default)
    {
        foreach (var item in order.OrderItems)
        {
            if (!_menu.Any(x => x.Id == item.Id))
            {
                return Task.FromResult(new SendOrderResponseDto(
                    false,
                    $"Блюда с id {item.Id} нет в меню")
                );
            }

            if (item.Quantity <= 0.0)
            {
                return Task.FromResult(new SendOrderResponseDto(
                    false,
                    $"Для блюда с id {item.Id} указано неверное количество {item.Quantity}, ожидалось больше нуля")
                );
            }
        }

        return Task.FromResult(new SendOrderResponseDto(
            true,
            string.Empty)
        );
    }
}