using System.Collections.Generic;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Dataport.Terminfinder.Console.DeleteAppointments.Tool;

/// <inheritdoc cref="IRepository" />
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
            throw new ApplicationException("The customerid can not be empty");
        }

        NpgsqlParameter paramCustomerId = new("@CUSTOMERID", NpgsqlDbType.Uuid);
        paramCustomerId.NpgsqlValue = customerId;
        NpgsqlParameter paramDeleteDate = new("@DELETEDATE", NpgsqlDbType.Date);
        paramDeleteDate.NpgsqlValue = deleteDate;
        NpgsqlParameter[] parameters = new NpgsqlParameter[] { paramCustomerId, paramDeleteDate };

        List<Appointment> appointmentsToDelete = new();

        using NpgsqlConnection connection = GetConnection(connectionString);
        using NpgsqlDataReader dr = ExecuteReader(connection, parameters, CmdSelectExpiresAppointments);
        while (dr.Read())
        {
            string appointmentId = dr["appointmentid"].ToString();

            if (!string.IsNullOrEmpty(appointmentId))
            {
                appointmentsToDelete.Add(new Appointment()
                {
                    AppointmentId = new(appointmentId), CustomerId = customerId
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

        using NpgsqlConnection connection = GetConnection(connectionString);
        foreach (Appointment appointmentToDelete in appointmentsToDelete)
        {
            _logger.LogDebug(
                "Delete appointment {AppointmentToDeleteAppointmentId}", appointmentToDelete.AppointmentId);

            NpgsqlParameter paramAppointmentIdToDelete =
                new("@APPOINTMENTID", NpgsqlDbType.Uuid);
            paramAppointmentIdToDelete.NpgsqlValue = appointmentToDelete.AppointmentId;

            NpgsqlParameter paramCustomerIdToDelete =
                new("@CUSTOMERID", NpgsqlDbType.Uuid);
            paramCustomerIdToDelete.NpgsqlValue = appointmentToDelete.CustomerId;

            NpgsqlParameter[] parametersDeleteAppointment = new NpgsqlParameter[]
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

        NpgsqlConnection connection = new(connectionString);
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
                $"Error {nameof(ExecuteReader)}: The connection are not open or the command are not defined.");
            throw new ApplicationException("The connection are not open or the command are not defined.");
        }

        NpgsqlCommand command = new(commandText, connection);

        if (parameters != null)
        {
            foreach (NpgsqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
        }

        NpgsqlDataReader dr = command.ExecuteReader();
        return dr;
    }

    private void ExecuteNonQuery(NpgsqlConnection connection, NpgsqlParameter[] parameters, string commandText)
    {
        _logger.LogDebug("Enter {NameofExecuteNonQuery)} Parameter: {Connection}, {Parameters}, {CommandText}",
            nameof(ExecuteNonQuery), connection, parameters, commandText);

        if (connection == null || string.IsNullOrEmpty(commandText))
        {
            _logger.LogError(
                "Error {NameofExecuteNonQuery}: The connection are not open or the command are not defined.",
                nameof(ExecuteNonQuery));
            throw new ApplicationException("The connection are not open or the command are not defined.");
        }

        NpgsqlCommand command = new(commandText, connection);

        if (parameters != null)
        {
            foreach (NpgsqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
        }

        int rows = command.ExecuteNonQuery();
        if (rows <= 0)
        {
            string error = $"No rows are deleted, parameters: {parameters}";
            _logger.LogError("Error {NameofExecuteNonQuery}: {Error}", nameof(ExecuteNonQuery), error);
            throw new ApplicationException(error);
        }
    }
}