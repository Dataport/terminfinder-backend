namespace Dataport.Terminfinder.Console.DeleteAppointments.Tool.Enum;

/// <summary>
/// Error numbers
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// No Error occurs
    /// </summary>
    NoError = 0,

    /// <summary>
    /// Not enough parameters
    /// </summary>
    NotEnoughParameters = 10,

    /// <summary>
    /// too many parameters
    /// </summary>
    TooManyParameters = 20,

    /// <summary>
    /// customerId are not valid
    /// </summary>
    CustomerIdAreNotValid = 30,

    /// <summary>
    /// deleteExpiredAppointmentsAfterDays are not an integer
    /// </summary>
    DeleteExpiredAppointmentsAfterDaysAreNotAnInteger = 40,

    /// <summary>
    /// deleteExpiredAppointmentsAfterDays has to be greater than zero
    /// </summary>
    DeleteExpiredAppointmentsAfterDaysHasToBeGreaterThanZero = 50,

    /// <summary>
    /// DbConnection is not defined
    /// </summary>
    DbConnectionIsNotDefined = 60,

    /// <summary>
    /// unexpected error
    /// </summary>
    UnexpectedError = 70
}