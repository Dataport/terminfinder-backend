﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Dataport.Terminfinder.Console.DeleteAppointments.Tool.Enum;

namespace Dataport.Terminfinder.Console.DeleteAppointments.Tool;

public class Program
{
    /// <summary>
    /// Main
    /// </summary>
    /// <param name="args">customerId, days after that expired appointments would be deleted</param>
    /// <example>dotnet TerminfinderDeleteAppointments.dll 5C075919-0374-4063-A2C7-3147C6A22C30 7'</example>
    /// <returns>ErrorTyp</returns>
    /// <see cref="ErrorType"/>
    static int Main(string[] args)
    {
        if (args is { Length: < 2 })
        {
            System.Console.WriteLine("Error: There are not enough parameters.");
            System.Console.WriteLine(
                "dotnet Dataport.Terminfinder.DeleteAppointments.dll customerid 'days after that expired appointments would be deleted'");
            return (int)ErrorType.NotEnoughParameters;
        }

        if (args is { Length: > 2 })
        {
            System.Console.WriteLine("Error: There are too much parameters.");
            System.Console.WriteLine(
                "dotnet Dataport.Terminfinder.DeleteAppointments.dll customerid 'days after that expired appointments would be deleted'");
            return (int)ErrorType.TooMuchParamters;
        }

        if (!Guid.TryParse(args[0], out Guid customerId))
        {
            System.Console.WriteLine(
                "Error: The customerId are not valid. Please use a format like this '5C075919-0374-4063-A2C7-3147C6A22C30'.");
            return (int)ErrorType.CustomerIdAreNotValid;
        }

        if (customerId == Guid.Empty)
        {
            System.Console.WriteLine(
                "Error: The customerId are not valid. Please use a format like this '5C075919-0374-4063-A2C7-3147C6A22C30'.");
            return (int)ErrorType.CustomerIdAreNotValid;
        }

        if (!int.TryParse(args[1], out int deleteExpiredAppointmentsAfterDays))
        {
            System.Console.WriteLine("Error: The second parameter days has to be a integer");
            return (int)ErrorType.DeleteExpiredAppointmentsAfterDaysAreNotAnInteger;
        }

        if (deleteExpiredAppointmentsAfterDays <= 0)
        {
            System.Console.WriteLine(
                $"The configurationvalue of deleteExpiredAppointmentsAfterDays has to be greater than zero.");
            return (int)ErrorType.DeleteExpiredAppointmentsAfterDaysHasToBeGreaterThanZero;
        }

        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddConsole();
                logging.AddDebug();
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
                services.AddTransient<IDeleteAppointmentsService, DeleteAppointmentsService>();
                services.AddTransient<IRepository, Repository>();
                services.AddLogging();
            })
            .Build();

        IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
        ILogger<Program> logger = host.Services.GetService<ILogger<Program>>();
        ILoggerFactory loggerFactory = host.Services.GetService<ILoggerFactory>();

        var log4NetFilename = config["Terminfinder:Log4NetConfigFilename"];

        if (!string.IsNullOrEmpty(log4NetFilename))
        {
            loggerFactory.AddLog4Net(log4NetFilename);
            logger?.LogDebug($"use log4Net-Filename: {log4NetFilename}");
        }
        else
        {
            loggerFactory.AddLog4Net();
        }

        string connectionString = config["ConnectionStrings:TerminfinderConnection"];
        logger?.LogDebug($"connectionString: {connectionString}");
        if (string.IsNullOrEmpty(connectionString))
        {
            logger?.LogError("The connectionstring is not defined");
            return (int)ErrorType.DbConnectionIsNotDefined;
        }

        try
        {
            logger?.LogDebug(
                "Enter {NameofMain} Parameter: {ConnectionString}, {DeleteExpiredAppointmentsAfterDays}",
                nameof(Main), connectionString, deleteExpiredAppointmentsAfterDays);

            IDateTimeGeneratorService dateTimeGeneratorService = new DateTimeGeneratorService();

            host.Services.GetService<IDeleteAppointmentsService>()?.DeleteExpiredAppointments(connectionString,
                customerId,
                deleteExpiredAppointmentsAfterDays, dateTimeGeneratorService);

            return (int)ErrorType.NoError;
        }
        catch (Exception e)
        {
            string error = $"An unexpected error was occurred: {e.Message} ; {e.StackTrace}";
            System.Console.WriteLine(error);
#pragma warning disable CA2254 // Template should be a static expression
            logger?.LogError(error);
#pragma warning restore CA2254 // Template should be a static expression
            return (int)ErrorType.UnexpectedError;
        }
        finally
        {
            logger?.LogDebug("Leave {NameofMain}", nameof(Main));
        }
    }
}