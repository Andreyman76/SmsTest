using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Sms.Test;
using SmsTest.Contracts;
using SmsTest.Domain;
using SmsTest.Domain.DTO;

namespace SmsTest.Client;

public class SmsTestRpcServiceClient(Uri uri) : ISmsTestServiceClient
{
    private readonly SmsTestService.SmsTestServiceClient _client =
        new(GrpcChannel.ForAddress(uri));

    public async Task<GetMenuResponseDto> GetMenuAsync(
        GetMenuRequestDto request,
        CancellationToken token = default)
    {
        var response = await _client.GetMenuAsync(
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
        var response = await _client.SendOrderAsync(
            request.ToProto(),
            cancellationToken: token);

        return response.ToDto();
    }
}