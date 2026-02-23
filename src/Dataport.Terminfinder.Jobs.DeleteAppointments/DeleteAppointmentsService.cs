using Dataport.Terminfinder.Common.Jobs;
using Dataport.Terminfinder.Common.Jobs.Configuration;
using Dataport.Terminfinder.Common.Services;
using Dataport.Terminfinder.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dataport.Terminfinder.Jobs.DeleteAppointments;

public class DeleteAppointmentsService(
    IOptions<DeleteAppointmentsConfig> options,
    IDateTimeGeneratorService dateTimeGeneratorService,
    IAppointmentRepository appointmentRepository,
    ILogger<DeleteAppointmentsService> logger) : IJob
{
    public Task ExecuteAsync()
    {
        logger.LogDebug(
            "Enter {ClassName}.{MethodName} with parameters: " +
            "customerId:'{CustomerId}', deleteExpiredAppointmentsAfterDays: '{DeleteExpiredAppointmentsAfterDays}'",
            nameof(DeleteAppointmentsService),
            nameof(ExecuteAsync),
            options.Value.CustomerId,
            options.Value.DeleteExpiredAppointmentsAfterDays);

        var deleteDate = dateTimeGeneratorService.GetCurrentDateTime();
        deleteDate = deleteDate.Subtract(TimeSpan.FromDays(options.Value.DeleteExpiredAppointmentsAfterDays + 1));

        var appointmentIdsToDelete =
            appointmentRepository.GetAppointmentIdsToDelete(options.Value.CustomerId, deleteDate);

        return appointmentIdsToDelete.Count == 0
            ? Task.CompletedTask
            : Task.FromResult(() => appointmentRepository.DeleteAppointmentsById(appointmentIdsToDelete));
    }
}