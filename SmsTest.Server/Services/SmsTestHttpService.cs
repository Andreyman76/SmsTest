using Microsoft.AspNetCore.Http;
using SmsTest.Contracts;
using SmsTest.Contracts.Http;
using System.Text.Json;

namespace SmsTest.Server.Services;

internal class SmsTestHttpService(
    ISmsTestService service)
{
    public async Task<IResult> HandleAsync(
        HttpRequest request,
        CancellationToken token = default)
    {
        try
        {
            var dto = await request.ReadFromJsonAsync<HttpRequestDto>(token);

            if (dto is null)
            {
                return CreateErrorResponse(
                    SmsTestApiCommands.FailedCommand,
                    "Некорректное тело запроса");
            }

            return dto.Command switch
            {
                SmsTestApiCommands.GetMenuCommand => await GetMenuAsync(
                    dto,
                    token),

                SmsTestApiCommands.SendOrderCommand => await SendOrderAsync(
                    dto,
                    token),

                _ => CreateErrorResponse(
                    dto.Command,
                    $"Неизвестная команда '{dto.Command}'")
            };
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(
                  SmsTestApiCommands.FailedCommand,
                  ex.Message);
        }
    }

    private async Task<IResult> GetMenuAsync(
        HttpRequestDto request,
        CancellationToken cancellationToken)
    {
        var parameters = request.CommandParameters
            .Deserialize<HttpGetMenuRequestDto>();

        if (parameters is null)
        {
            return CreateErrorResponse(
                SmsTestApiCommands.GetMenuCommand,
                "Некорректные параметры команды");
        }

        var response = await service.GetMenuAsync(
            parameters.ToDto(),
            cancellationToken);

        return Results.Json(response.ToHttpDto(),
            JsonSerializerOptions.Default,
            statusCode: 200);
    }

    private async Task<IResult> SendOrderAsync(
        HttpRequestDto request,
        CancellationToken cancellationToken)
    {
        var parameters = request.CommandParameters
            .Deserialize<HttpSendOrderRequestDto>();

        if (parameters is null)
        {
            return CreateErrorResponse(
                SmsTestApiCommands.SendOrderCommand,
                "Некорректные параметры команды");
        }

        var response = await service.SendOrderAsync(
            parameters.ToDto(),
            cancellationToken);

        return Results.Json(response.ToHttpDto(),
            JsonSerializerOptions.Default,
            statusCode: 200);
    }

    private static IResult CreateErrorResponse(
        string command,
        string errorMessage)
    {
        return Results.Json(new HttpErrorResponseDto(
            Command: command,
            Success: false,
            ErrorMessage: errorMessage
        ),
        JsonSerializerOptions.Default,
        statusCode: 200);
    }
}