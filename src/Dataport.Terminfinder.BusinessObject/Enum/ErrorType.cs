namespace Dataport.Terminfinder.BusinessObject.Enum;

/// <summary>
/// Error numbers
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Undefined status
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// general error
    /// </summary>
    GeneralError = 10,

    /// <summary>
    /// No Input, no parameters
    /// </summary>
    NoInput = 11,

    /// <summary>
    /// Wrong Input, parameters are wrong or not allowed
    /// </summary>
    WrongInputOrNotAllowed = 12,

    /// <summary>
    /// version number not found
    /// </summary>
    VersionNumberNotFound = 13,
    
    /// <summary>
    /// One of  the validated IDs is not empty
    /// </summary>
    IdsMustBeEmpty = 14,

    /// <summary>
    /// customerId not defined
    /// </summary>
    CustomerIdNotFound = 20,

    /// <summary>
    /// customerId not valid
    /// </summary>
    CustomerIdNotValid = 21,

    /// <summary>
    /// customer not valid
    /// </summary>
    CustomerNotValid = 22,

    /// <summary>
    /// customer not valid
    /// </summary>
    CustomerNotFound = 23,

    /// <summary>
    /// adminId not defined
    /// </summary>
    AdminIdNotFound = 30,

    /// <summary>
    /// adminId not valid
    /// </summary>
    AdminIdNotValid = 31,

    /// <summary>
    /// appointmentId not defined
    /// </summary>
    AppointmentIdNotFound = 40,

    /// <summary>
    /// appointmentId not valid
    /// </summary>
    AppointmentIdNotValid = 41,

    /// <summary>
    /// appointment not found
    /// </summary>
    AppointmentNotFound = 42,

    /// <summary>
    /// appointment not valid
    /// </summary>
    AppointmentNotValid = 43,

    /// <summary>
    /// The status of the appointment are not allowed in this context
    /// </summary>
    AppointmentStatusTypeNotAllowed = 44,

    /// <summary>
    /// The appointment has not be started
    /// </summary>
    AppointmentHasNotBeStarted = 45,

    /// <summary>
    /// the suggested date not found
    /// </summary>
    SuggestedDateNotFound = 50,
    
    /// <summary>
    /// suggestedDateId is not valid
    /// </summary>
    SuggestedDateIdNotValid = 51,

    /// <summary>
    /// participant not found
    /// </summary>
    ParticipantNotFound = 60,

    /// <summary>
    /// participantId not valid
    /// </summary>
    ParticipantIdNotValid = 61,

    /// <summary>
    /// participant not valid
    /// </summary>
    ParticipantNotValid = 62,

    /// <summary>
    /// the maximum count of elements of suggested dates are exceeded
    /// </summary>
    MaximumElementsOfSuggestedDatesAreExceeded = 70,

    /// <summary>
    /// the maximum count of elements of participants are exceeded
    /// </summary>
    MaximumElementsOfParticipantsAreExceeded = 71,

    /// <summary>
    /// the minimum count of elements of suggested dates are not exceeded
    /// </summary>
    MinimumElementsOfSuggestedDatesAreNotExceeded = 72,

    /// <summary>
    /// Authorization failed
    /// </summary>
    AuthorizationFailed = 100,

    /// <summary>
    /// A password is missing but it is required
    /// </summary>
    PasswordRequired = 101,

    /// <summary>
    /// An error occurred when decoding the password from the basic authentication header
    /// (it is a client error)
    /// </summary>
    DecodingPasswordFailed = 102
}