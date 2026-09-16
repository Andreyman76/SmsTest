using SmsTest.Domain.DTO;

namespace SmsTest.Domain;

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