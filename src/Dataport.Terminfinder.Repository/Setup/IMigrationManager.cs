namespace Dataport.Terminfinder.Repository.Setup;

/// <summary>
/// Migration database manager
/// </summary>
public interface IMigrationManager
{
    /// <summary>
    /// Update database
    /// </summary>
    public void MigrateDatabase();
}