
namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Web app status
/// </summary>
public enum WebAppStatus
{
    /// <summary>
    /// Configuration not available
    /// </summary>
    NotAvailable,

    /// <summary>
    /// Webserver undefined
    /// </summary>
    WebserverUndefined,

    /// <summary>
    /// Webserver not started
    /// </summary>
    WebserverNotStarted,

    /// <summary>
    /// Web app is running
    /// </summary>
    Running
}