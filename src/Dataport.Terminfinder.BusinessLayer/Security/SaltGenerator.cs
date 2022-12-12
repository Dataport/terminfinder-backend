namespace Dataport.Terminfinder.BusinessLayer.Security;

/// <summary>
/// Class for generating random salts
/// </summary>
public class SaltGenerator : ISaltGenerator
{
    private readonly ILogger _logger;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="logger"></param>
    public SaltGenerator(ILogger<SaltGenerator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public string GenerateSalt()
    {
        _logger.LogDebug($"Enter {nameof(GenerateSalt)}()");
        return BCrypt.Net.BCrypt.GenerateSalt();
    }
}