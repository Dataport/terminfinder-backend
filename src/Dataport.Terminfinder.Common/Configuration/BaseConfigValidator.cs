#nullable enable
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dataport.Terminfinder.Common.Configuration;

/// <summary>
/// BaseConfigValidator class
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="logger"></param>
public abstract class BaseConfigValidator<T>(ILogger logger) : IValidateOptions<T>
    where T : class
{
    /// <inheritdoc/>>
    public ValidateOptionsResult Validate(string? name, T? options)
    {
        var failures = new List<string>();

        if (options is null)
        {
            failures.Add("Configuration object is null.");
        }
        else
        {
            failures.AddRange(ValidateSpecific(name, options));
        }

        var result = failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;

        if (result.Succeeded)
        {
            logger.LogTrace("Config validation successful");
        }
        else
        {
            logger.LogError("Validation failed: {FailureMessage}", result.FailureMessage);
        }
        return result;
    }

    /// <summary>
    /// Validate the options object
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected abstract List<string> ValidateSpecific(string? name, T options);
}
