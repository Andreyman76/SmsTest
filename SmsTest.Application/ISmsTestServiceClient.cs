using SmsTest.Application.DTO;

namespace SmsTest.Application;

public interface ISmsTestServiceClient
{
    Task<GetMenuResponseDto> GetMenuAsync(
        GetMenuRequestDto request,
        CancellationToken token = default);

    Task<SendOrderResponseDto> SendOrderAsync(
        SendOrderRequestDto request,
        CancellationToken token = default
    );
}