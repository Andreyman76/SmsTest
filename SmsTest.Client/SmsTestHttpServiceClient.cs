using SmsTest.Application.Abstractions;
using SmsTest.Application.DTO;
using SmsTest.Contracts;
using SmsTest.Contracts.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmsTest.Client;

public class SmsTestHttpServiceClient(HttpClient client)
    : ISmsTestServiceClient
{
    public async Task<GetMenuResponseDto> GetMenuAsync(
        GetMenuRequestDto request,
        CancellationToken token = default)
    {
        var response = await client.PostAsJsonAsync(
            string.Empty,
            new HttpRequestDto(
                Command: SmsTestApiCommands.GetMenuCommand,
                CommandParameters: JsonSerializer.SerializeToElement(
                    request.ToHttpDto())
            ),
            token);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"{nameof(GetMenuAsync)} завершился с кодом ошибки: {response.StatusCode}");
        }

        var responseDto = await response.Content
            .ReadFromJsonAsync<HttpGetMenuResponseDto>(token);

        return responseDto?.ToDto()
            ?? throw new InvalidOperationException("Неверное тело ответа");
    }

    public async Task<SendOrderResponseDto> SendOrderAsync(
        SendOrderRequestDto request,
        CancellationToken token = default)
    {
        var response = await client.PostAsJsonAsync(
            string.Empty,
            new HttpRequestDto(
                Command: SmsTestApiCommands.SendOrderCommand,
                CommandParameters: JsonSerializer.SerializeToElement(
                    request.ToHttpDto())
            ),
            token);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"{nameof(SendOrderAsync)} завершился с кодом ошибки: {response.StatusCode}");
        }

        var responseDto = await response.Content
            .ReadFromJsonAsync<HttpSendOrderResponseDto>(token);

        return responseDto?.ToDto()
            ?? throw new InvalidOperationException("Неверное тело ответа");
    }
}