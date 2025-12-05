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
    /// DbConnection is not defined
    /// </summary>
    DbConnectionIsNotDefined = 10,

    /// <summary>
    /// unexpected error
    /// </summary>
    UnexpectedError = 20
}