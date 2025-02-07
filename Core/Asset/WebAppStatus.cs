
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
    /// Web server undefined
    /// </summary>
    WebServerUndefined,

    /// <summary>
    /// Web server not started
    /// </summary>
    WebServerNotStarted,

    /// <summary>
    /// Web app is running
    /// </summary>
    Running
}