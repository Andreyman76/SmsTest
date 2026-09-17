using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Sms.Test;
using SmsTest.Client;
using SmsTest.ConsoleApp.Configuration;
using SmsTest.ConsoleApp.DAL;
using SmsTest.ConsoleApp.Utilities;
using SmsTest.Domain;
using System.Net.Http.Headers;
using System.Text;

namespace SmsTest.ConsoleApp;

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var builder = Host.CreateApplicationBuilder(args);

            ConfigureServices(builder);

            using var host = builder.Build();
            using var scope = host.Services.CreateScope();

            using var context = scope.ServiceProvider
                .GetRequiredService<SmsTestDbContext>();

            context.Database.Migrate();

            var application = host.Services.GetRequiredService<ConsoleApplication>();

            await application.RunAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Приложение завершилось с критической ошибкой");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static void ConfigureServices(
       HostApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(
                    new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build())
                .CreateLogger();

        builder.Services.AddSerilog(Log.Logger);
        builder.Services.AddSingleton<LoggedConsole>();

        builder.Services
            .AddOptions<ServerOptions>()
            .BindConfiguration(ServerOptions.SectionName)
            .ValidateOnStart()
            .Validate(ValidateServerOptions);

        var protocol = builder.Configuration["Server:Protocol"];

        // Под это дело лучше завести фабрику
        if (string.Equals(protocol, "http", StringComparison.OrdinalIgnoreCase))
        {
            builder.Services
                .AddHttpClient<ISmsTestServiceClient, SmsTestHttpServiceClient>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<ServerOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(options.HttpServerUrl);

                    var credentials =
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes(
                                $"{options.Username}:{options.Password}"));

                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Basic",
                            credentials);
                });
        }
        else if (string.Equals(protocol, "grpc", StringComparison.OrdinalIgnoreCase))
        {
            builder.Services.AddGrpcClient<SmsTestService.SmsTestServiceClient>((serviceProvider, options) =>
            {
                var o = serviceProvider
                        .GetRequiredService<IOptions<ServerOptions>>()
                        .Value;

                options.Address = new Uri(o.GrpcServerUrl);
            });

            builder.Services.AddSingleton<ISmsTestServiceClient, SmsTestGrpcServiceClient>();
        }
        else
        {
            throw new NotImplementedException($"Неизвестный протокол: {protocol}");
        }

        builder.Services.AddDbContext<SmsTestDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("Database")
                ?? throw new InvalidOperationException(
                    "Строка подключения 'Database' не задана");

            options.UseNpgsql(connectionString);
        });

        builder.Services.AddSingleton<DishRepository>();
        builder.Services.AddTransient<ConsoleApplication>();
    }

    private static bool ValidateServerOptions(ServerOptions options)
    {
        if (string.Equals(options.Protocol, "http", StringComparison.OrdinalIgnoreCase))
        {
            return !string.IsNullOrWhiteSpace(options.HttpServerUrl)
                && !string.IsNullOrWhiteSpace(options.Username)
                && !string.IsNullOrWhiteSpace(options.Password);
        }

        if (string.Equals(options.Protocol, "grpc", StringComparison.OrdinalIgnoreCase))
        {
            return !string.IsNullOrWhiteSpace(options.GrpcServerUrl);
        }

        return false;
    }
}