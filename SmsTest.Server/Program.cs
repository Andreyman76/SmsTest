using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

        ConfigureServices(builder);

        using var app = builder.Build();

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

        app.MapGrpcService<SmsTestGrpcService>();

        await app.RunAsync();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.WebHost.UseKestrel();
        builder.Services.AddGrpc();
        builder.Services.AddGrpcReflection();
        builder.Services.AddSingleton<ISmsTestService, SmsTestServiceMockup>();
        builder.Services.AddSingleton<SmsTestHttpService>();
    }
}