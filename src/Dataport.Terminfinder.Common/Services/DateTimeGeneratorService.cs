using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.Common.Services;

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