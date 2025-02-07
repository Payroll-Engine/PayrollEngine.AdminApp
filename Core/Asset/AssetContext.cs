using PayrollEngine.AdminApp.Setting;
using PayrollEngine.AdminApp.WebServer;
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
    /// Web server service
    /// </summary>
    public IWebServerService WebServerService { get; init; }
}