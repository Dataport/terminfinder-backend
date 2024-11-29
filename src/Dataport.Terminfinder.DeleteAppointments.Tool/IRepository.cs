using JetBrains.Annotations;

namespace Dataport.Terminfinder.Console.DeleteAppointments.Tool;

/// <summary>
/// Database Repository
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Get a lost of appointments to delete
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="customerId"></param>
    /// <param name="deleteDate">delete after x days</param>
    /// <exception cref="ApplicationException">e.g. customerId is empty</exception>
    /// <returns>List of appointments to delete</returns>
    [NotNull]
    List<Appointment> GetListOfAppointmentsToDelete(string connectionString, Guid customerId,
        DateTime deleteDate);

    /// <summary>
    /// Delete appointments
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="customerId"></param>
    /// <param name="appointmentsToDelete">List of appointments to delete</param>
    void DeleteAppointments(string connectionString, Guid customerId, List<Appointment> appointmentsToDelete);
}