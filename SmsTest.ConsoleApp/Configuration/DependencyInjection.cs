using Microsoft.Extensions.DependencyInjection;
using Sms.Test;
using SmsTest.Application;
using SmsTest.Client;
using System.Net.Http.Headers;
using System.Text;

namespace SmsTest.ConsoleApp.Configuration;

internal static class DependencyInjection
{
    public static IServiceCollection AddSmsTestServiceClient(
        this IServiceCollection services, ServerOptions serverOptions)
    {
        if (string.Equals(serverOptions.Protocol, "http", StringComparison.OrdinalIgnoreCase))
        {
            // Регистрация как typed client
            services.AddHttpClient<ISmsTestServiceClient, SmsTestHttpServiceClient>(
                (_, client) =>
                {
                    client.BaseAddress = ValidateHttpServerOptions(serverOptions);

                    var credentials =
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes(
                                $"{serverOptions.Username}:{serverOptions.Password}"));

                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Basic",
                            credentials);
                });
        }
        else if (string.Equals(serverOptions.Protocol, "grpc", StringComparison.OrdinalIgnoreCase))
        {
            services.AddGrpcClient<SmsTestService.SmsTestServiceClient>(
                (_, options) =>
            {
                options.Address = ValidateGrpcServerOptions(serverOptions);
            });

            services.AddSingleton<ISmsTestServiceClient, SmsTestGrpcServiceClient>();
        }
        else
        {
            throw new NotImplementedException($"Неизвестный протокол: {serverOptions.Protocol}");
        }

        return services;
    }

    private static Uri ValidateHttpServerOptions(
        ServerOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.HttpServerUrl))
        {
            throw new InvalidOperationException(
                "Для HTTP необходимо указать Server:HttpServerUrl");
        }

        if (string.IsNullOrWhiteSpace(options.Username))
        {
            throw new InvalidOperationException(
                "Для HTTP необходимо указать Server:Username");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            throw new InvalidOperationException(
                "Для HTTP необходимо указать Server:Password");
        }

        if (!Uri.TryCreate(
                options.HttpServerUrl,
                UriKind.Absolute,
                out Uri? uri))
        {
            throw new InvalidOperationException(
                $"Некорректный HTTP URL: {options.HttpServerUrl}");
        }

        if (uri.Scheme != Uri.UriSchemeHttp &&
            uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException(
                $"HTTP URL должен использовать http или https: " +
                $"{options.HttpServerUrl}");
        }

        return uri;
    }

    private static Uri ValidateGrpcServerOptions(
        ServerOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.GrpcServerUrl))
        {
            throw new InvalidOperationException(
                "Для gRPC необходимо указать Server:GrpcServerUrl");
        }

        if (!Uri.TryCreate(
                options.GrpcServerUrl,
                UriKind.Absolute,
                out Uri? uri))
        {
            throw new InvalidOperationException(
                $"Некорректный gRPC URL: {options.GrpcServerUrl}");
        }

        if (uri.Scheme != Uri.UriSchemeHttp &&
            uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException(
                $"gRPC URL должен использовать http или https: " +
                $"{options.GrpcServerUrl}");
        }

        return uri;
    }
}