using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using SmsTest.Client;
using SmsTest.ConsoleApp.Configuration;
using SmsTest.ConsoleApp.DAL;
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

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(
                    new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build())
                .CreateLogger();

            builder.Services.AddSerilog();

            builder.Services
                .AddOptions<ServerOptions>()
                .BindConfiguration(ServerOptions.SectionName)
                .ValidateOnStart();

            var type = builder.Configuration["Protocol"];

            if (string.Equals(type, "http", StringComparison.OrdinalIgnoreCase))
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

            if (string.Equals(type, "grpc", StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.AddSingleton<ISmsTestServiceClient>((serviceProvider) =>
                {
                    var options = serviceProvider
                            .GetRequiredService<IOptions<ServerOptions>>()
                            .Value;

                    return new SmsTestRpcServiceClient(
                        new Uri(options.GrpcServerUrl));
                });
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

            using var host = builder.Build();

            using var context = host.Services.GetRequiredService<SmsTestDbContext>();
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
}