using Dataport.Terminfinder.Common.Configuration;
using Dataport.Terminfinder.Common.Jobs.Configuration;
using Dataport.Terminfinder.Repository;

namespace Dataport.Terminfinder.WebAPI.Services.Configuration;

/// <summary>
/// Validate values for DeleteAppointmentsConfig
/// </summary>
/// <param name="logger"></param>
public class DeleteAppointmentsConfigValidator(
    ICustomerRepository customerRepository,
    ILogger<DeleteAppointmentsConfigValidator> logger)
    : BaseConfigValidator<DeleteAppointmentsConfig>(logger)
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor

    private static readonly string Prefix = DeleteAppointmentsConfig.SettingsKey + ".";
    private const int DeleteExpiredAppointmentsAfterDaysMinValue = 0;
    private const int DeleteExpiredAppointmentsAfterDaysMaxValue = 365;

    /// <inheritdoc/>
    protected override List<string> ValidateSpecific(string name, DeleteAppointmentsConfig options)
    {
        var failures = new List<string>();

        if (!options.IsEnabled)
        {
            return failures;
        }

        failures.AddRange(ValidateConfigValuesForCustomerId(options));
        failures.AddRange(ValidateConfigValuesForDeleteDays(options));

        return failures;
    }

    private List<string> ValidateConfigValuesForCustomerId(DeleteAppointmentsConfig options)
    {
        var failures = new List<string>();

        if (options.CustomerId == Guid.Empty)
        {
            failures.Add($"Value '{options.CustomerId}' in property '{Prefix}{nameof(options.CustomerId)}' is an empty Guid.");
        }

        if (!customerRepository.ExistsCustomer(options.CustomerId))
        {
            failures.Add($"Value '{options.CustomerId}' in property '{Prefix}{nameof(options.CustomerId)}' is not a valid customer Guid.");
        }

        return failures;
    }

    private static List<string> ValidateConfigValuesForDeleteDays(DeleteAppointmentsConfig options)
    {
        var failures = new List<string>();

        failures.AddRange(ConfigValidatorUtils.ValidateRangeValue(
            $"{Prefix}{nameof(options.DeleteExpiredAppointmentsAfterDays)}",
            options.DeleteExpiredAppointmentsAfterDays,
            minInclusive: DeleteExpiredAppointmentsAfterDaysMinValue,
            maxInclusive: DeleteExpiredAppointmentsAfterDaysMaxValue));

        return failures;
    }
}