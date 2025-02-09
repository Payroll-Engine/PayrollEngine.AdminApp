
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
    /// Webserver not defined
    /// </summary>
    WebserverUndefined,

    /// <summary>
    /// Webserver not started
    /// </summary>
    WebserverNotStarted,

    /// <summary>
    /// Backend is running
    /// </summary>
    Running
}