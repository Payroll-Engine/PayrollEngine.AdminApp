namespace PayrollEngine.AdminApp.Webserver;

/// <summary>
/// Payroll Engine webserver configuration service
/// </summary>
public interface IWebserverConfigurationService
{
    /// <summary>
    /// Get connection timeout in seconds
    /// </summary>
    int GetConnectionTimeout();
}