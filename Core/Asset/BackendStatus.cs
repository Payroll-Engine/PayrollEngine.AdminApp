
namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// On-premise backend status
/// </summary>
public enum BackendStatus
{
    /// <summary>
    /// Configuration not available
    /// </summary>
    NotAvailable,

    /// <summary>
    /// Database not available
    /// </summary>
    DatabaseNotAvailable,

    /// <summary>
    /// Web server not defined
    /// </summary>
    WebServerUndefined,

    /// <summary>
    /// Web server not started
    /// </summary>
    WebServerNotStarted,

    /// <summary>
    /// Backend is running
    /// </summary>
    Running
}