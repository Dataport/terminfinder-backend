namespace Dataport.Terminfinder.BusinessLayer.Security;

/// <summary>
/// Interface for generating random salts
/// </summary>
public interface ISaltGenerator
{
    /// <summary>
    /// Generate a random salt
    /// </summary>
    /// <returns>the random salt generated</returns>
    string GenerateSalt();
}