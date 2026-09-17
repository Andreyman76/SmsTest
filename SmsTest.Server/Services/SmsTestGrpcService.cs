using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Sms.Test;
using SmsTest.Contracts;
using SmsTest.Domain.DTO;

namespace SmsTest.Server.Services;

internal class SmsTestGrpcService(SmsTestServiceMockup service)
    : SmsTestService.SmsTestServiceBase
{
    public override async Task<GetMenuResponse> GetMenu(
        BoolValue request,
        ServerCallContext context)
    {
        var response = await service.GetMenuAsync(
            new GetMenuRequestDto(
                WithPrice: request.Value),
            context.CancellationToken);

        return response.ToProto();
    }

    public override async Task<SendOrderResponse> SendOrder(
        Order request,
        ServerCallContext context)
    {
        var response = await service.SendOrderAsync(
            request.ToDto(),
            context.CancellationToken);

        return response.ToProto();
    }
}