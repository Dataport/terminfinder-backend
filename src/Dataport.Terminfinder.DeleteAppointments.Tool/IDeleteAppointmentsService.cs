namespace Dataport.Terminfinder.Console.DeleteAppointments.Tool;

public interface IDeleteAppointmentsService
{
    /// <summary>
    /// delete all appointments, if the oldest startdate (when enddate is null) or the oldest enddate older than x days
    /// </summary>
    /// <param name="connectionString">connectionString</param>
    /// <param name="customerId">customerId, e.g. '5C075919-0374-4063-A2C7-3147C6A22C30'</param>
    /// <param name="deleteExpiredAppointmentsAfterDays">x days after the last enddate or the last startdate, where no enddate, the appointments could be delete</param>
    /// <param name="dateTimeGeneratorService">dateTimeGeneratorService</param>
    /// <remarks>s. https://stackoverflow.com/questions/38510740/is-ado-net-in-net-core-possible </remarks>
    /// <exception cref="ApplicationException">the parameters are wrong</exception>
    void DeleteExpiredAppointments(string connectionString, Guid customerId, int deleteExpiredAppointmentsAfterDays,
        IDateTimeGeneratorService dateTimeGeneratorService);
}