using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;

namespace SmsTest.Server.Authentication;

internal sealed class BasicAuthenticationMiddleware(
    RequestDelegate next,
    IConfiguration configuration)
{
    private readonly string _username = configuration["Authentication:Username"]
            ?? throw new InvalidOperationException(
                "Authentication:Username is not configured.");
    private readonly string _password = configuration["Authentication:Password"]
            ?? throw new InvalidOperationException(
                "Authentication:Password is not configured.");

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path != "/api")
        {
            await next(context);
            return;
        }

        if (!TryAuthenticate(context.Request, out _))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.Headers.WWWAuthenticate = "Basic";

            return;
        }

        await next(context);
    }

    private bool TryAuthenticate(
        HttpRequest request,
        out string? username)
    {
        username = null;

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

        username = credentials[..separator];

        var password = credentials[(separator + 1)..];

        return string.Equals(
            username,
            _username,
            StringComparison.Ordinal)
            &&
        string.Equals(
            password,
            _password,
            StringComparison.Ordinal);
    }
}