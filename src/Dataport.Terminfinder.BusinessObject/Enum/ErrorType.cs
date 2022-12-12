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
    /// customerid not defined
    /// </summary>
    CustomerIdNotFound = 20,

    /// <summary>
    /// appointmentid not defined
    /// </summary>
    AppointmentIdNotFound = 30,

    /// <summary>
    /// adminid not defined
    /// </summary>
    AdminIdNotFound = 31,

    /// <summary>
    /// customer not valid
    /// </summary>
    CustomerNotValid = 40,

    /// <summary>
    /// customer not valid
    /// </summary>
    CustomerNotFound = 41,

    /// <summary>
    /// versionnumber not found
    /// </summary>
    VersionNumberNotFound = 50,

    /// <summary>
    /// appointment not found
    /// </summary>
    AppointmentNotFound = 60,

    /// <summary>
    /// appointment not valid
    /// </summary>
    AppointmentNotValid = 61,

    /// <summary>
    /// The status of the appointment are not allowed in this context
    /// </summary>
    AppointmentStatusTypeNotAllowed = 62,

    /// <summary>
    /// The appointment has not be started
    /// </summary>
    AppointmentHasNotBeStarted = 63,

    /// <summary>
    /// participant not valid
    /// </summary>
    ParticipantNotValid = 70,

    /// <summary>
    /// the maximum count of elements of suggested dates are exceeded
    /// </summary>
    MaximumElementsOfSuggestedDatesAreExceeded = 80,

    /// <summary>
    /// the maximum count of elements of participants are exceeded
    /// </summary>
    MaximumElementsOfParticipantsAreExceeded = 81,

    /// <summary>
    /// the minimum count of elements of suggested dates are not exceeded
    /// </summary>
    MinimumElementsOfSuggestedDatesAreNotExceeded = 82,

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
    DecodingPasswordFailed = 102,

    /// <summary>
    /// the suggested date not found
    /// </summary>
    SuggestedDateNotFound = 110,

    /// <summary>
    /// participant not found
    /// </summary>
    ParticipantNotFound = 120

}