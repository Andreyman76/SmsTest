using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using SmsTest.Wpf.Configuration;
using SmsTest.Wpf.Services;
using SmsTest.Wpf.ViewModels;
using SmsTest.Wpf.Views;
using System.Windows;
using System.Windows.Threading;

namespace SmsTest.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        try
        {
            DispatcherUnhandledException += HandleException;

            var builder = Host.CreateApplicationBuilder();
            ConfigureServices(builder);
            _host = builder.Build();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Приложение завершилось с критической ошибкой");
            Environment.Exit(-1);
        }
    }

    private void HandleException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Fatal(e.Exception, "Приложение завершилось с критической ошибкой");
    }

    private static void ConfigureServices(
        HostApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(
                builder.Configuration)
            .CreateLogger();

        builder.Services
            .AddSerilog(Log.Logger)
            .AddTransient<MainWindowViewModel>()
            .AddTransient<MainWindow>()

            .AddSingleton<IEnvironmentVariablesService>(serviceProvider =>
            {
                var options = serviceProvider
                          .GetRequiredService<IOptions<EnvironmentVariablesOptions>>()
                          .Value;

                return new EnvironmentVariablesService(
                    options.KnownVariables,
                    serviceProvider.GetService<ILogger>());
            })
            .AddOptions<EnvironmentVariablesOptions>()
                .BindConfiguration(EnvironmentVariablesOptions.SectionName)
                .ValidateOnStart()
                .Validate(ValidateEnvironmentVariables);
    }

    /// <summary>
    /// Проверка уникальности и наличия имен переменных
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private static bool ValidateEnvironmentVariables(
        EnvironmentVariablesOptions options)
    {
        var names = options.KnownVariables
            .Select(x => x.Name);

        if (names.Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() != names.Count())
        {
            return false;
        }

        if (names.Any(string.IsNullOrWhiteSpace))
        {
            return false;
        }

        return true;
    }

    protected override async void OnStartup(
        StartupEventArgs e)
    {
        await _host.StartAsync();

        var window = _host.Services.GetRequiredService<MainWindow>();

        window.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(
        ExitEventArgs e)
    {
        try
        {
            await _host.StopAsync();
            await Log.CloseAndFlushAsync();

            base.OnExit(e);
        }
        catch (Exception ex)
        {
            Log.Fatal(
                ex,
                "Приложение завершилось с критической ошибкой");

            throw;
        }
        finally
        {
            _host.Dispose();
        }
    }
}