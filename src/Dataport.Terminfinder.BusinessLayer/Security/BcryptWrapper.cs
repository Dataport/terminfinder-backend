namespace Dataport.Terminfinder.BusinessLayer.Security;

/// <summary>
/// Class for bcrypt functions
/// </summary>
public class BcryptWrapper : IBcryptWrapper
{
    private readonly ILogger _logger;
    private readonly ISaltGenerator _saltGenerator;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="saltGenerator"></param>
    /// <param name="logger"></param>
    public BcryptWrapper(ISaltGenerator saltGenerator, ILogger<BcryptWrapper> logger)
    {
        _saltGenerator = saltGenerator ?? throw new ArgumentNullException(nameof(saltGenerator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        _logger.LogDebug("Enter {NameofHashPassword}(string)", nameof(HashPassword));

        if (password == null) throw new ArgumentNullException(nameof(password));

        string salt = _saltGenerator.GenerateSalt();

        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    /// <inheritdoc />
    public bool Verify(string password, string passwordHash)
    {
        _logger.LogDebug("Enter {NameofVerify}(string), (string)", nameof(Verify));

        if (password == null) throw new ArgumentNullException(nameof(password));
        if (passwordHash == null) throw new ArgumentNullException(nameof(passwordHash));

        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
