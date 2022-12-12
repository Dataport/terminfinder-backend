using Dataport.Terminfinder.BusinessObject;

namespace Dataport.Terminfinder.Repository;

/// <inheritdoc cref="IAppConfigRepository" />
public class AppConfigRepository : RepositoryBase, IAppConfigRepository
{
    private readonly ILogger<AppConfigRepository> _logger;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="logger"></param>
    public AppConfigRepository(DataContext ctx, ILogger<AppConfigRepository> logger)
        : base(ctx)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public AppInfo GetAppInfo()
    {
        _logger.LogDebug($"Enter {nameof(GetAppInfo)}");

        AppInfo appVersion = null;

        string versionnr = (from config in Context.AppConfig
            where config.Key == "version"
            select config.Value).SingleOrDefault();

        string builddate = (from config in Context.AppConfig
            where config.Key == "builddate"
            select config.Value).SingleOrDefault();

        if (!string.IsNullOrEmpty(versionnr) && (!string.IsNullOrEmpty(builddate)))
        {
            appVersion = new AppInfo { VersionNumber = versionnr, BuildDate = builddate };
        }

        return appVersion;
    }
}