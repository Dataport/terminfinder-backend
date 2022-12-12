using System.Collections;

namespace Dataport.Terminfinder.BusinessObject.Validators;

/// <summary>
/// Check minimum elements 
/// </summary>
public class EnsureMinimumElementsAttribute : ValidationAttribute
{
    private static readonly int MinimumElements_MinimumValue = 1;
    private int _minimumElements = MinimumElements_MinimumValue;

    /// <summary>
    /// Minimum count of elements
    /// </summary>
    public int MinElements
    {
        get => _minimumElements;
        set
        {
            if (value < MinimumElements_MinimumValue)
                throw new ArgumentException($"The value is lesser than {MinimumElements_MinimumValue}");

            _minimumElements = value;
        }
    }

    /// <summary>
    /// Check if element is valid
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override bool IsValid(object value)
    {
        if (value is ICollection collection)
        {
            return collection.Count >= _minimumElements;
        }

        return false;
    }
}