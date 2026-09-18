using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using SmsTest.Wpf.Configuration;
using SmsTest.Wpf.Services;
using SmsTest.Wpf.ViewModels;
using System.Windows;

namespace SmsTest.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        var builder =
           Host.CreateApplicationBuilder();

        ConfigureServices(builder);

        _host = builder.Build();
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

        builder.Services
            .AddOptions<EnvironmentVariablesOptions>()
            .BindConfiguration(EnvironmentVariablesOptions.SectionName)
            .ValidateOnStart()
            .Validate(ValidateEnvironmentVariables);

        builder.Services.AddSingleton<IEnvironmentVariablesService>(serviceProvider =>
        {
            var options = serviceProvider
                      .GetRequiredService<IOptions<EnvironmentVariablesOptions>>()
                      .Value;

            return new EnvironmentVariablesService(
                options.KnownVariables,
                serviceProvider.GetService<ILogger>());
        });

        builder.Services.AddSingleton<MainWindowViewModel>();
        builder.Services.AddSingleton<MainWindow>();
    }

    /// <summary>
    /// Проверка уникальности и наличия имен переменных
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private static bool ValidateEnvironmentVariables(EnvironmentVariablesOptions options)
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
        await _host.StopAsync();
        await Log.CloseAndFlushAsync();

        _host.Dispose();

        base.OnExit(e);
    }
}