using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmsTest.Server.Services;

namespace FactoryMonitor.Server;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            EnvironmentName = Environments.Development
        });

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(5000, listenOptions =>
            {
                listenOptions.Protocols =
                    Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
            });
        });

        builder.Services.AddGrpc();
        builder.Services.AddGrpcReflection();

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapGrpcReflectionService();
        }

        app.MapGrpcService<SmsTestRpcServiceMockup>();

        await app.RunAsync();
    }
}