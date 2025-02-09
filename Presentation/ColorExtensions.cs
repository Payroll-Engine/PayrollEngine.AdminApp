using MudBlazor;
using PayrollEngine.AdminApp.Asset;
using PayrollEngine.AdminApp.Webserver;
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
            BackendStatus.WebserverUndefined => Color.Error,
            BackendStatus.WebserverNotStarted => Color.Error,
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
            WebAppStatus.WebserverUndefined => Color.Error,
            WebAppStatus.WebserverNotStarted => Color.Error,
            WebAppStatus.Running => Color.Success,
            _ => Color.Default
        };

    /// <summary>
    /// Get webserver edit color
    /// </summary>
    /// <param name="status">Webserver status</param>
    public static Color ToEditColor(this WebserverStatus status) =>
        status switch
        {
            WebserverStatus.UndefinedConnection => Color.Error,
            WebserverStatus.NotAvailable => Color.Error,
            WebserverStatus.Available => Color.Success,
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
            DatabaseStatus.MissingDatabase => Color.Default,
            DatabaseStatus.EmptyDatabase => Color.Default,
            DatabaseStatus.OutdatedVersion => Color.Default,
            DatabaseStatus.Available => Color.Success,
            _ => Color.Default
        };
}