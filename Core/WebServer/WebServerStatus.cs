namespace PayrollEngine.AdminApp.WebServer;

/// <summary>
/// Web server status
/// </summary>
public enum WebServerStatus
{
    /// <summary>
    /// Undefined web server connection
    /// </summary>
    UndefinedConnection,

    /// <summary>
    /// Web server is not available
    /// </summary>
    NotAvailable,

    /// <summary>
    /// Web server is available
    /// </summary>
    Available
}