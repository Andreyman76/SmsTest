using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Sms.Test;
using SmsTest.Contracts;
using SmsTest.Application.DTO;

namespace SmsTest.Server.Services;

internal class SmsTestGrpcService(
    ISmsTestService service)
    : SmsTestService.SmsTestServiceBase
{
    public override async Task<GetMenuResponse> GetMenu(
        BoolValue request,
        ServerCallContext context)
    {
        try
        {
            var response = await service.GetMenuAsync(
                new GetMenuRequestDto(
                    WithPrice: request.Value),
                context.CancellationToken);

            return response.ToProto();
        }
        catch (Exception ex)
        {
            return new GetMenuResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public override async Task<SendOrderResponse> SendOrder(
        Order request,
        ServerCallContext context)
    {
        try
        {
            var response = await service.SendOrderAsync(
                request.ToDto(),
                context.CancellationToken);

            return response.ToProto();
        }
        catch (Exception ex)
        {
            return new SendOrderResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}