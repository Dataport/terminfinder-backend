namespace Dataport.Terminfinder.Console.DeleteAppointments.Tool;

/// <summary>
/// Gets the current DateTime
/// </summary>
public interface IDateTimeGeneratorService
{
    /// <summary>
    /// Gets the current DateTime
    /// </summary>
    DateTime GetCurrentDateTime();
}