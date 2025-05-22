using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.Repository.Setup;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
public class MigrationManager : IMigrationManager
{

    private readonly ILogger<MigrationManager> _logger;

    private readonly DataContext _dataContext;

    /// <summary>
    /// Update database
    /// </summary>
    public MigrationManager(
        ILogger<MigrationManager> logger,
        DataContext dataContext)
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    /// <inheritdoc />
    public void MigrateDatabase()
    {
        try
        {
            if (_dataContext.Database.GetPendingMigrations().Any())
            {
                _dataContext.Database.Migrate();
                _logger.LogInformation("Migration completed successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Migration failed {ex}", ex);
            throw;
        }
    }
}