using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.Common.Jobs.Configuration;

/// <summary>
/// Configure Delete Appointments Job
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteAppointmentsConfig
{
    /// <summary>
    /// Settings key in appsettings file
    /// </summary>
    public static readonly string SettingsKey = "DeleteAppointments";
    /// <summary>
    /// Default setting for days to wait before deletion of expired appointments
    /// </summary>
    public static readonly int DefaultDeleteExpiredAppointmentsAfterDays = 7;

    /// <summary>
    /// CustomerId to delete appointments for
    /// </summary>
    public Guid CustomerId { get; set; } = Guid.Empty;
    /// <summary>
    /// Days to wait before deletion of expired appointments
    /// </summary>
    public int DeleteExpiredAppointmentsAfterDays { get; set; } = DefaultDeleteExpiredAppointmentsAfterDays;
}