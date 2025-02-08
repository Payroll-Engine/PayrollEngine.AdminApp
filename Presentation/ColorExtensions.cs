using MudBlazor;
using PayrollEngine.AdminApp.Asset;
using PayrollEngine.AdminApp.WebServer;
using PayrollEngine.AdminApp.Persistence;

namespace PayrollEngine.AdminApp.Presentation;

/// <summary>
/// Color extension methods for object status
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Get backend edit color
    /// </summary>
    /// <param name="status">Backend status</param>
    public static Color ToEditColor(this BackendStatus status) =>
        status switch
        {
            BackendStatus.NotAvailable => Color.Error,
            BackendStatus.DatabaseNotAvailable => Color.Error,
            BackendStatus.WebServerUndefined => Color.Error,
            BackendStatus.WebServerNotStarted => Color.Error,
            BackendStatus.Running => Color.Success,
            _ => Color.Default
        };

    /// <summary>
    /// Get web app edit color
    /// </summary>
    /// <param name="status">Web app status</param>
    public static Color ToEditColor(this WebAppStatus status) =>
        status switch
        {
            WebAppStatus.NotAvailable => Color.Error,
            WebAppStatus.WebServerUndefined => Color.Error,
            WebAppStatus.WebServerNotStarted => Color.Error,
            WebAppStatus.Running => Color.Success,
            _ => Color.Default
        };

    /// <summary>
    /// Get web server edit color
    /// </summary>
    /// <param name="status">Web server status</param>
    public static Color ToEditColor(this WebServerStatus status) =>
        status switch
        {
            WebServerStatus.UndefinedConnection => Color.Error,
            WebServerStatus.NotAvailable => Color.Error,
            WebServerStatus.Available => Color.Success,
            _ => Color.Default
        };

    /// <summary>
    /// Get database edit color
    /// </summary>
    /// <param name="status">Database status</param>
    public static Color ToEditColor(this DatabaseStatus status) =>
        status switch
        {
            DatabaseStatus.UndefinedConnection => Color.Error,
            DatabaseStatus.InvalidConnection => Color.Error,
            DatabaseStatus.MissingServer => Color.Error,
            DatabaseStatus.MissingDatabase => Color.Error,
            DatabaseStatus.EmptyDatabase => Color.Error,
            DatabaseStatus.OutdatedVersion => Color.Error,
            DatabaseStatus.Available => Color.Success,
            _ => Color.Default
        };
}