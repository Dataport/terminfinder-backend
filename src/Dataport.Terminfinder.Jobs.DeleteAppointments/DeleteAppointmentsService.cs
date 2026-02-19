using Dataport.Terminfinder.Common.Jobs;
using Dataport.Terminfinder.Common.Services;
using Dataport.Terminfinder.Repository;
using Microsoft.Extensions.Logging;

namespace Dataport.Terminfinder.Jobs.DeleteAppointments;

public class DeleteAppointmentsService(
    IDateTimeGeneratorService dateTimeGeneratorService,
    IAppointmentRepository appointmentRepository,
    ILogger<DeleteAppointmentsService> logger) : IJob
{
    // TODO refactor zu appsettings/config
    private readonly Guid _customerId = Guid.Parse("80248A42-8FE2-4D4A-89DA-02E683511F76");
    private const int DeleteExpiredAppointmentsAfterDays = 7;

    public Task ExecuteAsync()
    {
        logger.LogDebug(
            "Enter {ClassName}.{MethodName} with parameters: " +
            "customerId:'{CustomerId}', deleteExpiredAppointmentsAfterDays: '{DeleteExpiredAppointmentsAfterDays}'",
            nameof(DeleteAppointmentsService),
            nameof(ExecuteAsync),
            _customerId,
            DeleteExpiredAppointmentsAfterDays);
        
        var deleteDate = dateTimeGeneratorService.GetCurrentDateTime();
        deleteDate = deleteDate.Subtract(TimeSpan.FromDays(DeleteExpiredAppointmentsAfterDays + 1));

        var appointmentIdsToDelete = appointmentRepository.GetAppointmentIdsToDelete(_customerId, deleteDate);

        return appointmentIdsToDelete.Count == 0
            ? Task.CompletedTask
            : Task.FromResult(() => appointmentRepository.DeleteAppointmentsById(appointmentIdsToDelete));
    }
}