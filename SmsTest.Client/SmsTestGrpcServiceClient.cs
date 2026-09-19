using Google.Protobuf.WellKnownTypes;
using Sms.Test;
using SmsTest.Application;
using SmsTest.Application.DTO;
using SmsTest.Contracts;

namespace SmsTest.Client;

public class SmsTestGrpcServiceClient(
    SmsTestService.SmsTestServiceClient client)
    : ISmsTestServiceClient
{
    public async Task<GetMenuResponseDto> GetMenuAsync(
        GetMenuRequestDto request,
        CancellationToken token = default)
    {
        var response = await client.GetMenuAsync(
            new BoolValue
            {
                Value = request.WithPrice
            },
            cancellationToken: token);

        return response.ToDto();
    }

    public async Task<SendOrderResponseDto> SendOrderAsync(
        SendOrderRequestDto request,
        CancellationToken token = default)
    {
        var response = await client.SendOrderAsync(
            request.ToProto(),
            cancellationToken: token);

        return response.ToDto();
    }
}