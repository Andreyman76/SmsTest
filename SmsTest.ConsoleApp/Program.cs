using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SmsTest.ConsoleApp.Configuration;
using SmsTest.ConsoleApp.DAL;
using SmsTest.ConsoleApp.Utilities;

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

            var application = host.Services.GetRequiredService<ConsoleApplication>();

            using (var scope = host.Services.CreateScope())
            {
                using var context = scope.ServiceProvider
                    .GetRequiredService<SmsTestDbContext>();

                context.Database.Migrate();
            }

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

        builder.Services
            .AddSerilog(Log.Logger)
            .AddSingleton<IConsole, LoggingConsole>()
            .AddSmsTestServiceClient(builder.Configuration
                .GetSection(ServerOptions.SectionName)
                .Get<ServerOptions>() ?? throw new InvalidOperationException($"{nameof(ServerOptions)} не задан"))
            .AddSingleton<DishRepository>()
            .AddTransient<ConsoleApplication>()
            .AddDbContext<SmsTestDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("Database")
                    ?? throw new InvalidOperationException(
                        "Строка подключения 'Database' не задана");

                options.UseNpgsql(connectionString);
            });
    }
}