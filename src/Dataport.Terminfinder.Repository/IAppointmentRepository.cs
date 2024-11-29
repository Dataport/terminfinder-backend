using Dataport.Terminfinder.BusinessObject;

namespace Dataport.Terminfinder.Repository;

/// <summary>
/// Business methods for appointments
/// </summary>
public interface IAppointmentRepository : IRepositoryBase
{
    /// <summary>
    /// Read appointment by its id and by the customer id from db
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <returns>Appointment or null if it does not exist</returns>
    [CanBeNull]
    Appointment GetAppointment(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Read appointment by its adminid and by the customer id from db
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="adminId">adminId of the appointment</param>
    /// <returns>Appointment or null if it does not exist</returns>
    [CanBeNull]
    Appointment GetAppointmentByAdminId(Guid customerId, Guid adminId);

    /// <summary>
    /// Read adminId from appointment with id from db for Admin
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <returns>adminid or guid.empty if customer or appointmentid not found or valid</returns>
    Guid GetAppointmentAdminId(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Add and update new appointment to context
    /// </summary>
    /// <param name="appointment"></param>
    void AddAndUpdateAppointment(Appointment appointment);

    /// <summary>
    /// Check, if the appointment exists
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="appointmentId"></param>
    /// <returns>true, if the customer appointment exists otherwise false</returns>
    bool ExistsAppointment(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Check, if the appointment exists
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="appointmentId"></param>
    /// <param name="adminId"></param>
    /// <returns>true, if the customer appointment exists otherwise false</returns>
    bool ExistsAppointment(Guid customerId, Guid appointmentId, Guid adminId);

    /// <summary>
    /// Check, if the appointment exists and the status is started
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="appointmentId"></param>
    /// <returns>true, if the appointment exists and is started otherwise false</returns>
    bool ExistsAppointmentIsStarted(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Check, if the appointment exists (appointment identified by admin id)
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="adminId"></param>
    /// <returns>true, if the customer appointment exists otherwise false</returns>
    bool ExistsAppointmentByAdminId(Guid customerId, Guid adminId);

    /// <summary>
    /// Check, if participant exists in the database
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    /// <param name="participantId">if of the participant</param>
    /// <returns>true, if participant exists, otherwise false</returns>
    bool ExistsParticipant(Guid customerId, Guid appointmentId, Guid participantId);

    /// <summary>
    /// Read parcipiant with there votings for one appointment from db
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <returns>Lists of participants and their votings</returns>
    [CanBeNull]
    ICollection<Participant> GetParticipants(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Add and update participants with voting to context
    /// </summary>
    /// <param name="participants"></param>
    void AddAndUpdateParticipants(ICollection<Participant> participants);

    /// <summary>
    /// Delete participian with voting from context
    /// </summary>
    /// <param name="participant">participant to delete</param>
    void DeleteParticipant(Participant participant);

    /// <summary>
    /// Get number of all suggestedDates of the appointment
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <returns>Number of suggestedDates</returns>
    int GetNumberOfSuggestedDates(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Get number of all participants of the appointment
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <returns>Number of participants</returns>
    int GetNumberOfParticipants(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Check, if suggested date exists in the database
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    /// <param name="suggestedDateId">id of the suggested date</param>
    /// <returns>true, if the suggested date exists, otherwise false</returns>
    bool ExistsSuggestedDate(Guid customerId, Guid appointmentId, Guid suggestedDateId);

    /// <summary>
    /// Delete suggested date with votings from context
    /// </summary>
    /// <param name="suggestedDate">suggested date to delete</param>
    void DeleteSuggestedDate(SuggestedDate suggestedDate);

    /// <summary>
    /// Read the password from the appointment
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    /// <returns>the password</returns>
    /// <exception cref="InvalidOperationException">will be thrown if the appointment does not exist</exception>
    [CanBeNull]
    string GetAppointmentPassword(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Read the password from the appointment (appointment identified by admin id)
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="adminId">adminId of the appointment</param>
    /// <returns>the password</returns>
    /// <exception cref="InvalidOperationException">will be thrown if the appointment does not exist</exception>
    [CanBeNull]
    string GetAppointmentPasswordByAdmin(Guid customerId, Guid adminId);

    /// <summary>
    /// Get the status of the appointment (appointment identified by admin id)
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="adminId">adminId of the appointment</param>
    /// <returns>Status of the appiontment</returns>
    /// <exception cref="InvalidOperationException">will be thrown if the appointment does not exist</exception>
    [NotNull]
    string GetAppointmentStatusTypeByAdmin(Guid customerId, Guid adminId);

    /// <summary>
    /// Set the new status of the appointment (appointment identified by admin id)
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="adminId">adminId of the appointment</param>
    /// <param name="statusIdentifier">new StatusType</param>
    /// <exception cref="InvalidOperationException">will be thrown if the appointment does not exist</exception>
    void SetAppointmentStatusTypeByAdmin(Guid customerId, Guid adminId, string statusIdentifier);
}