using System.Text.Json;

namespace SmsTest.Contracts.Http;

public sealed record HttpRequestDto(
    string Command,
    JsonElement CommandParameters
);