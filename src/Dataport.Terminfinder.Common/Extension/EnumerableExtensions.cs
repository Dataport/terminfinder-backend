namespace Dataport.Terminfinder.Common.Extension;

/// <summary>
/// Extension methods for interface IEnumerable
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Determines whether the collection is null or contains no elements.
    /// </summary>
    /// <remarks>
    /// Code from https://stackoverflow.com/questions/8582344/does-c-sharp-have-isnullorempty-for-list-ienumerable/8582374#8582374
    /// </remarks>
    /// <typeparam name="T">The IEnumerable type.</typeparam>
    /// <param name="enumerable">The enumerable, which may be null or empty.</param>
    /// <returns>
    ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
    /// </returns>
    [ContractAnnotation("enumerable:null => true")]
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
        /* If this is a list, use the Count property for efficiency. The Count property is O(1) while IEnumerable.Count() is O(N). */
        return enumerable switch
        {
            null => true,
            ICollection<T> collection => collection.Count < 1,
            _ => !enumerable.Any()
        };
    }
}