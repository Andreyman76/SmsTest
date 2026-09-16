using System.Text.Json;

namespace SmsTest.Contracts.Http;

public sealed class HttpRequestDto
{
    public string Command { get; set; } = string.Empty;
    public JsonElement CommandParameters { get; set; }
}