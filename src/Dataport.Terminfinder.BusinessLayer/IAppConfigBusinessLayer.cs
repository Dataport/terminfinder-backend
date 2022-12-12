using Dataport.Terminfinder.BusinessObject;

namespace Dataport.Terminfinder.BusinessLayer;

/// <summary>
/// BusinessLayer of AppConfig
/// </summary>
public interface IAppConfigBusinessLayer
{
    /// <summary>
    /// Version
    /// </summary>
    /// <returns>versionnumber and builddate</returns>
    AppInfo GetAppInfo();
}