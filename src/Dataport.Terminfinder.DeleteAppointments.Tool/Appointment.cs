using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.Console.DeleteAppointments.Tool;

/// <summary>
/// Information about one appointment to delete
/// </summary>
[ExcludeFromCodeCoverage]
public class Appointment
{
    /// <summary>
    /// CustomerId
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// AppointmentId
    /// </summary>
    public Guid AppointmentId { get; set; }
}