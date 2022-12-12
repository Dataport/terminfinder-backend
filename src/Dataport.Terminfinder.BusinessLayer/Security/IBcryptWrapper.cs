namespace Dataport.Terminfinder.BusinessLayer.Security;

/// <summary>
/// Interface for bcrypt functions
/// </summary>
public interface IBcryptWrapper
{
    /// <summary>
    /// Create the hash of the password
    /// </summary>
    /// <param name="password">the passsword to create the hash from</param>
    /// <returns>the created hash</returns>
    [NotNull]
    string HashPassword(string password);

    /// <summary>
    /// Verify the password and the password hash.
    /// </summary>
    /// <param name="password">the passsword</param>
    /// <param name="passwordHash">the hash</param>
    /// <returns>true if the verification of the password and the hash was successful, otherwise false.</returns>
    bool Verify(string password, string passwordHash);
}