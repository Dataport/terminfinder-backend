using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.Console.DeleteAppointments.Tool;

/// <inheritdoc cref="IRepository" />
[ExcludeFromCodeCoverage]
public class Repository : IRepository
{
    private readonly ILogger _logger;

    private static readonly string CmdSelectExpiresAppointments =
        @"select appointment.appointmentid
from appointment join suggesteddate on
(
	appointment.appointmentid = suggesteddate.appointmentid
and appointment.customerid = suggesteddate.customerid
)
where appointment.customerid = @CUSTOMERID
and suggesteddate.startdate is not null
group by appointment.appointmentid
having(max(suggesteddate.enddate) < @DELETEDATE::date)
and not exists
(
	select appointment.appointmentid
    from appointment as appointment2 join suggesteddate as suggesteddate2 on
	(
		appointment2.appointmentid = suggesteddate2.appointmentid
	and appointment2.customerid = suggesteddate2.customerid
	)
	where appointment2.appointmentid = appointment.appointmentid
    and suggesteddate2.enddate is null
    group by appointment.appointmentid
	having(max(suggesteddate2.startdate) >= @DELETEDATE::date)
)
union
select appointment.appointmentid
from appointment join suggesteddate on
(
		appointment.appointmentid = suggesteddate.appointmentid
	and appointment.customerid = suggesteddate.customerid
)
where appointment.customerid = @CUSTOMERID
and suggesteddate.enddate is null
group by appointment.appointmentid
having(max(suggesteddate.startdate) < @DELETEDATE::date)
and not exists
(
	select appointment.appointmentid
    from appointment as appointment2 join suggesteddate as suggesteddate2 on
	(
    	appointment2.appointmentid = suggesteddate2.appointmentid
		and appointment2.customerid = suggesteddate2.customerid
	)
	where appointment2.appointmentid = appointment.appointmentid
	group by appointment.appointmentid
    having(max(suggesteddate2.enddate) >= @DELETEDATE::date)
);";

    private static readonly string CmdDeleteExpiredAppointment =
        @"delete from appointment where customerId = @CUSTOMERID and appointmentid = @APPOINTMENTID;";

    /// <summary>
    /// Database Repository
    /// </summary>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException">logger</exception>
    public Repository(ILogger<Repository> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public List<Appointment> GetListOfAppointmentsToDelete(string connectionString, Guid customerId,
        DateTime deleteDate)
    {
        _logger.LogDebug(
            "Enter {NameofGetListOfAppointmentsToDelete} Parameter: {ConnectionString}, {DeleteDate}, {CustomerId}",
            nameof(GetListOfAppointmentsToDelete), connectionString, deleteDate, customerId);

        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("The customerid can not be empty");
        }

        var paramCustomerId = new NpgsqlParameter("@CUSTOMERID", NpgsqlDbType.Uuid) { NpgsqlValue = customerId };
        var paramDeleteDate = new NpgsqlParameter("@DELETEDATE", NpgsqlDbType.Date) { NpgsqlValue = deleteDate };
        var parameters = new[] { paramCustomerId, paramDeleteDate };

        var appointmentsToDelete = new List<Appointment>();

        using var connection = GetConnection(connectionString);
        using var dr = ExecuteReader(connection, parameters, CmdSelectExpiresAppointments);
        while (dr.Read())
        {
            var appointmentId = dr["appointmentid"].ToString();

            if (!string.IsNullOrEmpty(appointmentId))
            {
                appointmentsToDelete.Add(new Appointment()
                {
                    AppointmentId = new Guid(appointmentId), CustomerId = customerId
                });
            }
        }

        return appointmentsToDelete;
    }

    /// <inheritdoc />
    public void DeleteAppointments(string connectionString, Guid customerId, List<Appointment> appointmentsToDelete)
    {
        _logger.LogDebug(
            "Enter {NameofDeleteAppointments} Parameter: {ConnectionString}, {CustomerId}",
            nameof(DeleteAppointments), connectionString, customerId);

        using var connection = GetConnection(connectionString);
        foreach (var appointmentToDelete in appointmentsToDelete)
        {
            _logger.LogDebug(
                "Delete appointment {AppointmentToDeleteAppointmentId}", appointmentToDelete.AppointmentId);

            var paramAppointmentIdToDelete = new NpgsqlParameter("@APPOINTMENTID", NpgsqlDbType.Uuid) { NpgsqlValue = appointmentToDelete.AppointmentId };

            var paramCustomerIdToDelete = new NpgsqlParameter("@CUSTOMERID", NpgsqlDbType.Uuid) { NpgsqlValue = appointmentToDelete.CustomerId };

            var parametersDeleteAppointment = new[]
            {
                paramAppointmentIdToDelete, paramCustomerIdToDelete
            };
            ExecuteNonQuery(connection, parametersDeleteAppointment, CmdDeleteExpiredAppointment);
        }
    }

    private NpgsqlConnection GetConnection(string connectionString)
    {
        _logger.LogDebug("Enter {NameofGetConnection} Parameter: {ConnectionString}", nameof(GetConnection),
            connectionString);

        var connection = new NpgsqlConnection(connectionString);
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        return connection;
    }

    private NpgsqlDataReader ExecuteReader(NpgsqlConnection connection, NpgsqlParameter[] parameters,
        string commandText)
    {
        _logger.LogDebug("Enter {NameofExecuteReader} Parameter: {Connection}, {Parameters}, {CommandText}",
            nameof(ExecuteReader), connection, parameters, commandText);

        if (connection == null || string.IsNullOrEmpty(commandText))
        {
            _logger.LogError(
                $"Error {nameof(ExecuteReader)}: The connection ist not open or the command is not defined.");
            throw new ArgumentException("The connection ist not open or the command is not defined.");
        }

        var command = new NpgsqlCommand(commandText, connection);

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
        }

        var dr = command.ExecuteReader();
        return dr;
    }

    private void ExecuteNonQuery(NpgsqlConnection connection, NpgsqlParameter[] parameters, string commandText)
    {
        _logger.LogDebug("Enter {NameofExecuteNonQuery} Parameter: {Connection}, {Parameters}, {CommandText}",
            nameof(ExecuteNonQuery), connection, parameters, commandText);

        if (connection == null || string.IsNullOrEmpty(commandText))
        {
            _logger.LogError(
                "Error {NameofExecuteNonQuery}: The connection ist not open or the command is not defined.",
                nameof(ExecuteNonQuery));
            throw new ArgumentException("The connection ist not open or the command is not defined.");
        }

        var command = new NpgsqlCommand(commandText, connection);

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
        }

        var rows = command.ExecuteNonQuery();
        if (rows <= 0)
        {
            var error = $"No rows are deleted, parameters: {parameters}";
            _logger.LogError("Error {NameofExecuteNonQuery}: {Error}", nameof(ExecuteNonQuery), error);
            throw new ArgumentException(error);
        }
    }
}