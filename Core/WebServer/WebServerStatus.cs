namespace PayrollEngine.AdminApp.Webserver;

/// <summary>
/// Webserver status
/// </summary>
public enum WebserverStatus
{
    /// <summary>
    /// Undefined webserver connection
    /// </summary>
    UndefinedConnection,

    /// <summary>
    /// Webserver is not available
    /// </summary>
    NotAvailable,

    /// <summary>
    /// Webserver is available
    /// </summary>
    Available
}