using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmsTest.Server.Authentication;
using SmsTest.Server.Services;

namespace SmsTest.Server;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            EnvironmentName = Environments.Development
        });

        var grpcPort = builder.Configuration.GetValue<int>("Server:GrpcPort");
        var httpPort = builder.Configuration.GetValue<int>("Server:HttpPort");

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(grpcPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });

            options.ListenLocalhost(httpPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1;
            });
        });

        builder.Services.AddGrpc();
        builder.Services.AddGrpcReflection();
        builder.Services.AddSingleton<SmsTestServiceMockup>();
        builder.Services.AddSingleton<SmsTestHttpService>();

        WebApplication app = builder.Build();

        app.UseMiddleware<BasicAuthenticationMiddleware>();

        app.MapPost("/api", async (
            HttpRequest request,
            SmsTestHttpService service,
            CancellationToken cancellationToken) =>
            {
                return await service.HandleAsync(
                    request,
                    cancellationToken);
            });

        if (app.Environment.IsDevelopment())
        {
            app.MapGrpcReflectionService();
        }

        app.MapGrpcService<SmsTestRpcService>();

        await app.RunAsync();
    }
}