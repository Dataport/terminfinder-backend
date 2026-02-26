namespace Dataport.Terminfinder.Common.Configuration;

/// <summary>
/// Util class for validation of config values
/// </summary>
public static class ConfigValidatorUtils
{
    private const string RangeHasToBeBetweenTemplate = "Value '{0}' in property '{1}' has to be between {2} and {3}.";
    private const string RangeHasToBeLessOrEqualThanTemplate = "Value '{0}' in property '{1}' has to be less or equal than {2}.";
    private const string RangeHasToBeEqualOrGreaterThanTemplate = "Value '{0}' in property '{1}' has to be greater or equal than {2}.";

    /// <summary>
    /// Validates if the value is within a range.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    /// <param name="minInclusive"></param>
    /// <param name="maxInclusive"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IEnumerable<string> ValidateRangeValue(string propertyName, int? value, int? minInclusive = null,
        int? maxInclusive = null)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        var failures = new List<string>();

        if (minInclusive.HasValue && maxInclusive.HasValue && minInclusive.Value > maxInclusive.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(minInclusive),
                $"Value in {nameof(minInclusive)} has to be less or equal than in value in {nameof(maxInclusive)}");
        }

        if (value.HasValue)
        {
            if (minInclusive.HasValue && maxInclusive.HasValue && (value < minInclusive || value > maxInclusive))
            {
                failures.Add(
                    string.Format(RangeHasToBeBetweenTemplate, value, propertyName, minInclusive, maxInclusive));
            }
            else if (value > maxInclusive)
            {
                failures.Add(string.Format(RangeHasToBeLessOrEqualThanTemplate, value, propertyName, maxInclusive));
            }
            else if (value < minInclusive)
            {
                failures.Add(string.Format(RangeHasToBeEqualOrGreaterThanTemplate, value, propertyName, minInclusive));
            }
        }

        return failures;
    }
}