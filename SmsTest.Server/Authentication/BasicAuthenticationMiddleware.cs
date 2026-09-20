using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SmsTest.Server.Configuration;
using System.Net.Http.Headers;
using System.Text;

namespace SmsTest.Server.Authentication;

/// <summary>
/// Middleware базовой аутентификации
/// </summary>
/// <param name="next"></param>
/// <param name="configuration"></param>
internal class BasicAuthenticationMiddleware(
    RequestDelegate next,
    IOptions<AuthenticationOptions> options)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path != "/api")
        {
            await next(context);
            return;
        }

        if (!TryAuthenticate(context.Request))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.Headers.WWWAuthenticate = "Basic";

            return;
        }

        await next(context);
    }

    private bool TryAuthenticate(
        HttpRequest request)
    {
        if (!request.Headers.TryGetValue(
                "Authorization",
                out var authorization))
        {
            return false;
        }

        if (!AuthenticationHeaderValue.TryParse(
                authorization,
                out var header))
        {
            return false;
        }

        if (!string.Equals(
                header.Scheme,
                "Basic",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(header.Parameter))
        {
            return false;
        }

        byte[] bytes;

        try
        {
            bytes = Convert.FromBase64String(header.Parameter);
        }
        catch (FormatException)
        {
            return false;
        }

        var credentials = Encoding.UTF8.GetString(bytes);
        var separator = credentials.IndexOf(':');

        if (separator < 0)
        {
            return false;
        }

        var username = credentials[..separator];
        var password = credentials[(separator + 1)..];

        return string.Equals(
            username,
            options.Value.Username,
            StringComparison.Ordinal)
            &&
        string.Equals(
            password,
            options.Value.Password,
            StringComparison.Ordinal);
    }
}