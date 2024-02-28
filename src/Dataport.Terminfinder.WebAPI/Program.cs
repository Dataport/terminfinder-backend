using Dataport.Terminfinder.Repository.Setup;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dataport.Terminfinder.WebAPI;

/// <summary>
/// Program
/// </summary>
public static class Program
{
    /// <summary>
    /// Main
    /// </summary>
    /// <param name="args"></param>
    public static async Task Main(string[] args)
    {
        var hostBuilder = CreateHostBuilder(args).Build();
        if (args.Contains("--dbmigrate"))
        {
            using var scope = hostBuilder.Services.CreateScope();
            var migrationManager = scope.ServiceProvider.GetService<IMigrationManager>();
            if (migrationManager == null)
            {
                throw new ApplicationException("Database Migration failed!");
            }

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