using Dataport.Terminfinder.Common.Jobs.Configuration;
using Dataport.Terminfinder.Jobs.DeleteAppointments;
using Hangfire;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.WebAPI.Services;

/// <summary>
/// Service to handle registration of Hangfire jobs
/// </summary>
/// <param name="recurringJobManager"></param>
/// <param name="deleteAppointmentsConfig"></param>
/// <param name="logger"></param>
[ExcludeFromCodeCoverage]
public class HangfireJobRegistrationService(
    IRecurringJobManager recurringJobManager,
    IOptions<DeleteAppointmentsConfig> deleteAppointmentsConfig,
    ILogger<HangfireJobRegistrationService> logger)
{
    /// <summary>
    /// Register Hangfire jobs
    /// </summary>
    public void RegisterJobs()
    {
        if (deleteAppointmentsConfig.Value is { IsEnabled: true })
        {
            logger.LogInformation("The job '{JobKey}' is enabled and will run at every full hour.",
                DeleteAppointmentsConfig.JobKey);

            recurringJobManager.AddOrUpdate<DeleteAppointmentsService>(
                DeleteAppointmentsConfig.JobKey,
                deleteAppointmentsService => deleteAppointmentsService.ExecuteAsync(),
                Cron.HourInterval(1)
            );
        }
        else
        {
            logger.LogInformation("The job '{JobKey}' is disabled.", DeleteAppointmentsConfig.JobKey);

            recurringJobManager.RemoveIfExists(DeleteAppointmentsConfig.JobKey);
        }
    }
}