using System.Collections;

namespace Dataport.Terminfinder.BusinessObject.Validators;

/// <summary>
/// Check minimum elements 
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class EnsureMaximumElementsAttribute : ValidationAttribute
{
    private static readonly int MaximumElements_MaximumValue = 10000;
    private int _maxElements = MaximumElements_MaximumValue;

    /// <summary>
    /// Maximum count of elements
    /// </summary>
    public int MaxElements
    {
        get => _maxElements;
        set
        {
            if (value > MaximumElements_MaximumValue)
                throw new ArgumentException($"The value is greater than {MaximumElements_MaximumValue}");

            _maxElements = value;
        }
    }

    /// <summary>
    /// Check if element is valid
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override bool IsValid(object value)
    {
        return value switch
        {
            null => true,
            ICollection collection => collection.Count <= _maxElements,
            _ => false
        };
    }
}