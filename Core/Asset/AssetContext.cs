using PayrollEngine.AdminApp.Setting;
using PayrollEngine.AdminApp.Webserver;
using PayrollEngine.AdminApp.Persistence;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Asset context
/// </summary>
public class AssetContext
{
    /// <summary>
    /// Application settings
    /// </summary>
    public ISettingsService SettingsService { get; init; }

    /// <summary>
    /// Database service
    /// </summary>
    public IDatabaseService DatabaseService { get; init; }

    /// <summary>
    /// Webserver service
    /// </summary>
    public IWebserverService WebserverService { get; init; }
}