using Dataport.Terminfinder.Repository.Setup;
using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.WebAPI;

/// <summary>
/// Program
/// </summary>
[ExcludeFromCodeCoverage]
public static class Program
{
    /// <summary>
    /// Main
    /// </summary>
    /// <param name="args"></param>
    public static async Task Main(string[] args)
    {
        var hostBuilder = CreateHostBuilder(args).Build();

        using var scope = hostBuilder.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetService<IConfiguration>();

        if (args.Contains("--dbmigrate") || (configuration?.GetValue<string>("Terminfinder:dbmigrate") == "true"))
        {
            var migrationManager = scope.ServiceProvider.GetRequiredService<IMigrationManager>();

            migrationManager.MigrateDatabase();
        }

        await hostBuilder.RunAsync();
    }

    /// <summary>
    /// Build web host
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddConsole();
                logging.AddDebug();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(serviceOptions => { serviceOptions.AddServerHeader = false; })
                    .UseStartup<Startup>();
            });
}