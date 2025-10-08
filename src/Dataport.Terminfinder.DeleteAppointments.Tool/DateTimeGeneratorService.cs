using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.Console.DeleteAppointments.Tool;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
public class DateTimeGeneratorService : IDateTimeGeneratorService
{
    /// <inheritdoc />
    public DateTime GetCurrentDateTime()
    {
        return DateTime.Now.Date;
    }
}