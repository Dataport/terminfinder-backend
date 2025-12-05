using Dataport.Terminfinder.Console.DeleteAppointments.Tool.Enum;
using Dataport.Terminfinder.Repository.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.Repository.Migrate;

/// <summary>
/// MigrateDatabase
/// </summary>
[ExcludeFromCodeCoverage]
public static class MigrateDatabase
{
    /// <summary>
    /// Configuration
    /// </summary>
    public static IConfiguration Configuration { get; set; }
    
    public static int Main(string[] args)
    {
        using var host = Host
            .CreateDefaultBuilder(args)
            .ConfigureLogging(logger =>
            {
                logger.ClearProviders();
                logger.SetMinimumLevel(LogLevel.Trace);
                logger.AddConsole();
                logger.AddDebug();
            })
            .ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.Sources.Clear();

                configuration
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

                configuration.AddEnvironmentVariables();
            })
            .ConfigureServices(services =>
            {
                services.AddDbContext<DataContext>(options =>
                {
                    /*
                     * TODO connection muss über die config abgerufen werden
                     * existiert aber erst nach build
                     */
                    options.UseNpgsql(connectionString);
                });
                
                services.AddTransient<IMigrationManager, MigrationManager>();
                services.AddLogging();
            })
            .Build();

        var config = host.Services.GetRequiredService<IConfiguration>();
        var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(MigrateDatabase));

        var log4NetFilename = config["Terminfinder:Log4NetConfigFilename"];
        if (!string.IsNullOrEmpty(log4NetFilename))
        {
            loggerFactory.AddLog4Net(log4NetFilename);
            logger.LogDebug("use log4Net-Filename: {Log4NetFilename}", log4NetFilename);
        }
        else
        {
            loggerFactory.AddLog4Net();
        }

        var connectionString = config["ConnectionStrings:TerminfinderConnection"];
        logger.LogDebug("connectionString: {ConnectionString}", connectionString);
        if (string.IsNullOrEmpty(connectionString))
        {
            logger.LogError("The connection string is not defined");
            return (int)ErrorType.DbConnectionIsNotDefined;
        }

        try
        {
            var migrationManager = host.Services.GetRequiredService<IMigrationManager>();
            migrationManager.MigrateDatabase();

            return (int)ErrorType.NoError;
        }
        catch (Exception e)
        {
            var error = $"An unexpected error was occurred: {e.Message} ; {e.StackTrace}";
#pragma warning disable CA2254 // Template should be a static expression
            logger.LogError(e, error);
#pragma warning restore CA2254 // Template should be a static expression
            return (int)ErrorType.UnexpectedError;
        }
        finally
        {
            logger.LogDebug("Leave {NameofMain}", nameof(Main));
        }
    }
}