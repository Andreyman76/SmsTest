using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Sms.Test;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmsTest.Server.Services;

internal class SmsTestRpcServiceMockup : SmsTestService.SmsTestServiceBase
{
    public override async Task<GetMenuResponse> GetMenu(BoolValue request, ServerCallContext context)
    {
        var resposne = new GetMenuResponse();

        resposne.Success = true;

        var item = new MenuItem()
        {
            Id = "5979224",
            Article = "A1004292",
            Name = "Каша гречневая",
            Price = 50,
            IsWeighted = false,
            FullPath = "ПРОИЗВОДСТВО\\Гарниры",
        };

        item.Barcodes.Add("57890975627974236429");

        resposne.MenuItems.Add(item);

        resposne.MenuItems.Add(new MenuItem()
        {
            Id = "9084246",
            Article = "A1004293",
            Name = "Конфеты Коровка",
            Price = 300,
            IsWeighted = true,
            FullPath = "ДЕСЕРТЫ\\Развес"
        });

        return resposne;
    }

    public override async Task<SendOrderResponse> SendOrder(Order request, ServerCallContext context)
    {
        var response = new SendOrderResponse();

        response.Success = true;

        return response;
    }
}