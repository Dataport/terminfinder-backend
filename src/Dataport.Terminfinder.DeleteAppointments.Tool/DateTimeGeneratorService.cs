namespace Dataport.Terminfinder.Console.DeleteAppointments.Tool;

/// <inheritdoc />
public class DateTimeGeneratorService : IDateTimeGeneratorService
{
    /// <inheritdoc />
    public DateTime GetCurrentDateTime()
    {
        return DateTime.Now.Date;
    }
}