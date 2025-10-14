namespace Dataport.Terminfinder.Console.DeleteAppointments.Tool;

/// <summary>
/// delete all appointments, if the oldest startdate (when enddate is null) or the oldest enddate older than x days
/// </summary>
/// <remarks>s. https://stackoverflow.com/questions/38510740/is-ado-net-in-net-core-possible </remarks>
public class DeleteAppointmentsService : IDeleteAppointmentsService
{
    private readonly ILogger _logger;
    private readonly IRepository _databaseRepository;

    /// <summary>
    /// delete all appointments, if the oldest startdate (when enddate is null) or the oldest enddate older than x days
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException">repository</exception>
    /// <exception cref="ArgumentNullException">logger</exception>
    public DeleteAppointmentsService(IRepository repository, ILogger<DeleteAppointmentsService> logger)
    {
        _databaseRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public void DeleteExpiredAppointments(string connectionString, Guid customerId,
        int deleteExpiredAppointmentsAfterDays, IDateTimeGeneratorService dateTimeGeneratorService)
    {
        _logger.LogDebug(
            "Enter {NameofDeleteExpiredAppointments} Parameter: {ConnectionString}, {CustomerId}, {DeleteExpiredAppointmentsAfterDays}",
            nameof(DeleteExpiredAppointments), connectionString, customerId, deleteExpiredAppointmentsAfterDays);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("The connection string is not defined.");
        }

        if (deleteExpiredAppointmentsAfterDays <= 0)
        {
            throw new ArgumentException(
                "The configuration value of deleteExpiredAppointmentsAfterDays has to be greater than zero.");
        }

        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("The customerId is empty.");
        }

        var deleteDate = dateTimeGeneratorService.GetCurrentDateTime();
        deleteDate = deleteDate.Subtract(TimeSpan.FromDays(deleteExpiredAppointmentsAfterDays + 1));

        _logger.LogDebug("Delete all appointments till {DeleteDate}.", deleteDate);

        var appointmentsToDelete =
            _databaseRepository.GetListOfAppointmentsToDelete(connectionString, customerId, deleteDate);

        if (appointmentsToDelete.Count > 0)
        {
            _logger.LogDebug("There are {AppointmentsToDeleteCount} appointments to delete.",
                appointmentsToDelete.Count);
            _databaseRepository.DeleteAppointments(connectionString, customerId, appointmentsToDelete);
        }
    }
}