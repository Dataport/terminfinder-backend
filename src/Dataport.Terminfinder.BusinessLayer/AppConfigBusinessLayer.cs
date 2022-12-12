using Dataport.Terminfinder.Repository;
using Dataport.Terminfinder.BusinessObject;

namespace Dataport.Terminfinder.BusinessLayer;

/// <inheritdoc cref="IAppConfigBusinessLayer" />
public class AppConfigBusinessLayer : BusinessLayerBase, IAppConfigBusinessLayer
{
    private readonly IAppConfigRepository _appConfigRepository;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="appConfigRepository">AppConfig repository</param>
    /// <param name="logger">Logger</param>
    public AppConfigBusinessLayer(IAppConfigRepository appConfigRepository,
        ILogger<AppConfigBusinessLayer> logger)
        : base(logger, null)
    {
        Logger.LogDebug($"Enter {nameof(AppConfigBusinessLayer)}");

        _appConfigRepository = appConfigRepository;
    }

    /// <inheritdoc />
    public AppInfo GetAppInfo()
    {
        Logger.LogDebug($"Enter {nameof(GetAppInfo)}");
        return _appConfigRepository.GetAppInfo();
    }
}