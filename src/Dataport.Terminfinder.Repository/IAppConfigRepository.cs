using Dataport.Terminfinder.BusinessObject;

namespace Dataport.Terminfinder.Repository;

/// <summary>
/// Business methods for application
/// </summary>
public interface IAppConfigRepository : IRepositoryBase
{
    /// <summary>
    /// Version
    /// </summary>
    /// <returns>versionnumber and builddate</returns>
    AppInfo GetAppInfo();
}