using System.Collections.Generic;
using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;

namespace Dataport.Terminfinder.BusinessLayer;

/// <summary>
/// BusinessLayer of Appointment
/// </summary>
public interface IAppointmentBusinessLayer
{
    /// <summary>
    /// Check the total count of elements before the elements added to the database to check the maximum count of elements
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <param name="participants">Collection of participants to add or modify to/in the database</param>
    /// <result>True, if less or equal than allowed elements are added or in the database, otherwise false</result>
    bool CheckMaxTotalCountOfParticipants(Guid customerId, Guid appointmentId, ICollection<Participant> participants);

    /// <summary>
    /// check the total count of elements before the elements added to the database to check the minimum count of elements
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <param name="suggestedDates">Collection of suggested dates to add or modify to/in the database</param>
    /// <result>True, if greater or equal than allowed elements are added or in the database, otherwise false</result>
    bool CheckMinTotalCountOfSuggestedDates(Guid customerId, Guid appointmentId,
        ICollection<SuggestedDate> suggestedDates);

    /// <summary>
    /// check the total count of elements before the elements deleted in the database to check the minimum count of elements
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <param name="suggestedDates">Collection of suggested dates to delete in the database</param>
    /// <result>True, if greater or equal than allowed elements are delete or in the database, otherwise false</result>
    bool CheckMinTotalCountOfSuggestedDatesWithToDeletedDates(Guid customerId, Guid appointmentId,
        ICollection<SuggestedDate> suggestedDates);

    /// <summary>
    /// check the total count of elements before the elements added to the database to check the maximum count of elements
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <param name="suggestedDates">Collection of suggested dates to add or modify to/in the database</param>
    /// <result>True, if less or equal than allowed elements are added or in the database, otherwise false</result>
    bool CheckMaxTotalCountOfSuggestedDates(Guid customerId, Guid appointmentId,
        ICollection<SuggestedDate> suggestedDates);

    /// <summary>
    /// Check, if the customer is valid and has the status 'Started'
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns>true, if the customer is valid otherwise false</returns>
    bool ExistsCustomer(Guid customerId);

    /// <summary>
    /// Check, if the customer is valid and has the status 'Started'
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns>true, if the customer is valid otherwise false</returns>
    bool ExistsCustomer(string customerId);

    /// <summary>
    /// Check, if the appointment exists
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="appointmentId"></param>
    /// <returns>true, if the customer appointment exists otherwise false</returns>
    bool ExistsAppointment(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Check, if the appointment exists and if the submitted adminId is equal than the adminId of the existing appointment
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="appointmentId"></param>
    /// <param name="adminId"></param>
    /// <returns>true, if the customer appointment exists and the submitted adminId is equal than the adminId of the existing appointment; otherwise false</returns>
    bool ExistsAppointment(Guid customerId, Guid appointmentId, Guid adminId);

    /// <summary>
    /// Check, if the appointment exists and the status is started
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="appointmentId"></param>
    /// <returns>true, if the appointment exists and is started otherwise false</returns>
    bool ExistsAppointmentIsStarted(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Check, if the appointment exists (appointment is identified by admin id)
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="adminId"></param>
    /// <returns>true, if the customer appointment exists otherwise false</returns>
    bool ExistsAppointmentByAdminId(Guid customerId, Guid adminId);

    /// <summary>
    /// Read appointment by its id and by the customer id from db
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <returns>Appointment or null if it does not exist</returns>
    [CanBeNull]
    Appointment GetAppointment(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Detect if the appointment identified by customer id and appointment id is protected by a password
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <returns>true if the appointment is protected by a password; otherwise false</returns>
    /// <exception cref="InvalidOperationException">will be thrown if the appointment does not exist</exception>
    bool IsAppointmentPasswordProtected(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Read appointment by its adminid and by the customer id from db
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="adminId">adminId of the appointment</param>
    /// <returns>Appointment or null if it does not exist</returns>
    [CanBeNull]
    Appointment GetAppointmentByAdminId(Guid customerId, Guid adminId);

    /// <summary>
    /// Detect if the appointment identified by customer id and admin id is protected by a password
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="adminId">adminId of the appointment</param>
    /// <returns>true if the appointment is protected by a password; otherwise false</returns>
    /// <exception cref="InvalidOperationException">will be thrown if the appointment does not exist</exception>
    bool IsAppointmentPasswordProtectedByAdminId(Guid customerId, Guid adminId);

    /// <summary>
    /// Add new appointment to context
    /// </summary>
    /// <param name="appointment"></param>
    /// <returns>Appointment read from the database</returns>
    Appointment AddAppointment(Appointment appointment);

    /// <summary>
    /// Update appointment to context
    /// </summary>
    /// <param name="appointment"></param>
    /// <returns>Appointment read from the database</returns>
    Appointment UpdateAppointment(Appointment appointment);

    /// <summary>
    /// Update appointment to context
    /// </summary>
    /// <param name="appointment"></param>
    /// <param name="adminId"></param>
    /// <returns>Appointment read from the database or null, if adminId is empty</returns>
    Appointment UpdateAppointment(Appointment appointment, Guid adminId);

    /// <summary>
    /// Set foreign keys
    /// </summary>
    /// <param name="appointment"></param>
    /// <param name="customerId">id of the customer</param>
    void SetAppointmentForeignKeys(Appointment appointment, Guid customerId);

    /// <summary>
    /// save data context
    /// </summary>
    /// <returns>result of SaveChanges()</returns>
    int SaveAppointment();

    /// <summary>
    /// Check, if participant exists in the database
    /// </summary>
    /// <param name="customerId">id fo the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    /// <param name="participantId">if of the participant</param>
    /// <returns>true, if participant exists, otherwise false</returns>
    bool ExistsParticipant(Guid customerId, Guid appointmentId, Guid participantId);

    /// <summary>
    /// Read parcipiant with there votings for one appointment from db
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <returns>List of particpants and their votings</returns>
    [CanBeNull]
    ICollection<Participant> GetParticipants(Guid customerId, Guid appointmentId);

    /// <summary>
    /// Add and update participians with voting to context
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <param name="participants"></param>
    /// <returns>Collection of all participants in den database for the appointmentId</returns>
    ICollection<Participant> AddAndUpdateParticipiants(Guid customerId, Guid appointmentId,
        ICollection<Participant> participants);

    /// <summary>
    /// Delete participian with voting from context
    /// </summary>
    /// <param name="participant">participant to delete</param>
    void DeleteParticipiant(Participant participant);

    /// <summary>
    /// Check if all participant are valid
    /// </summary>
    /// <param name="participants">participants</param>
    /// <returns>true, if all participants are valid</returns>
    bool ParticipantsAreValid(ICollection<Participant> participants);

    /// <summary>
    /// Check if participant are valid
    /// </summary>
    /// <param name="participant">participant</param>
    /// <returns>true, if participant are valid</returns>
    bool ParticipantToDeleteAreValid(Participant participant);

    /// <summary>
    /// Check, if suggested date exists in the database
    /// </summary>
    /// <param name="customerId">id fo the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    /// <param name="suggestedDateId">id of the suggested date</param>
    /// <returns>true, if the suggested date exists, otherwise false</returns>
    bool ExistsSuggestedDate(Guid customerId, Guid appointmentId, Guid suggestedDateId);


    /// <summary>
    /// Delete suggested date with voting from context
    /// </summary>
    /// <param name="suggestedDate">suggestedDate to delete</param>
    void DeleteSuggestedDate(SuggestedDate suggestedDate);

    /// <summary>
    /// Check if suggested date is valid
    /// </summary>
    /// <param name="suggestedDate">suggested date</param>
    /// <returns>true, if suggested date is valid</returns>
    bool SuggestedDateToDeleteAreValid(SuggestedDate suggestedDate);

    /// <summary>
    /// set foreign keys
    /// </summary>
    /// <param name="participants">List of participants</param>
    /// <param name="customerId">id of the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    void SetParticipantsForeignKeys(ICollection<Participant> participants, Guid customerId, Guid appointmentId);

    /// <summary>
    /// set foreign keys
    /// </summary>
    /// <param name="suggestedDates">List of suggestedDates</param>
    /// <param name="customerId">id of the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    void SetSuggestedDatesForeignKeys(ICollection<SuggestedDate> suggestedDates, Guid customerId, Guid appointmentId);

    /// <summary>
    /// Try to verify the submitted password with the password from the appointment identified by the customer id and the appointment id.
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">adminId of the appointment</param>
    /// <param name="password">password to verify</param>
    /// <returns>true if the verification was successful; otherwise false</returns>
    bool VerifyAppointmentPassword(Guid customerId, Guid appointmentId, string password);

    /// <summary>
    /// Try to verify the submitted password with the password from the appointment identified by the customer id and the admin id.
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="adminId">adminId of the appointment</param>
    /// <param name="password">password to verify</param>
    /// <returns>true if the verification was successful; otherwise false</returns>
    bool VerifyAppointmentPasswordByAdminId(Guid customerId, Guid adminId, string password);

    /// <summary>
    /// Set the appointStatusType to the new value
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="adminId"></param>
    /// <param name="newStatusType">new appointmentStatusType</param>
    /// <returns>updated appointment if successfull, otherwise null</returns>
    [CanBeNull]
    Appointment SetAppointmentStatusType(Guid customerId, Guid adminId, AppointmentStatusType newStatusType);
}