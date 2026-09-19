using SmsTest.Application.DTO;

namespace SmsTest.Server.Services;

/// <summary>
/// Сервис для работы с меню и заказами
/// </summary>
internal interface ISmsTestService
{
    Task<GetMenuResponseDto> GetMenuAsync(
        GetMenuRequestDto request,
        CancellationToken token = default);

    Task<SendOrderResponseDto> SendOrderAsync(
        SendOrderRequestDto order,
        CancellationToken token = default);
}