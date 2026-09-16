using SmsTest.Contracts;
using SmsTest.Contracts.Http;
using SmsTest.Domain;
using SmsTest.Domain.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace SmsTest.Client;

public class SmsTestHttpServiceClient : ISmsTestServiceClient, IDisposable
{
    private readonly HttpClient _client;

    public SmsTestHttpServiceClient(
        Uri uri,
        string username,
        string password)
    {
        var credentials = Convert.ToBase64String(
           Encoding.UTF8.GetBytes($"{username}:{password}"));

        _client = new()
        {
            BaseAddress = uri
        };

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);
    }

    public async Task<GetMenuResponseDto> GetMenuAsync(
        GetMenuRequestDto request,
        CancellationToken token = default)
    {
        var response = await _client.PostAsJsonAsync(
            string.Empty,
            request.ToHttpDto(),
            token);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"{nameof(GetMenuAsync)} завершился с кодом ошибки: {response.StatusCode}");
        }

        var responseDto = await response.Content.ReadFromJsonAsync<HttpGetMenuResponseDto>(token);

        return responseDto?.ToDto()
            ?? throw new InvalidOperationException("Неверное тело ответа");
    }

    public async Task<SendOrderResponseDto> SendOrderAsync(
        SendOrderRequestDto request,
        CancellationToken token = default)
    {
        var response = await _client.PostAsJsonAsync(
            string.Empty,
            request.ToHttpDto(),
            token);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"{nameof(SendOrderAsync)} завершился с кодом ошибки: {response.StatusCode}");
        }

        var responseDto = await response.Content.ReadFromJsonAsync<HttpSendOrderResponseDto>(token);

        return responseDto?.ToDto()
            ?? throw new InvalidOperationException("Неверное тело ответа");
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}